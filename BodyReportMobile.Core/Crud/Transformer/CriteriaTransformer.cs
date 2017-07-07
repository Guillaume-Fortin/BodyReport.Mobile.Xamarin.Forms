﻿using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using BodyReport.Framework;

namespace BodyReportMobile.Core.Crud.Transformer
{
    public static class CriteriaTransformer
    {
        public static void CompleteQuery<TEntity, TCriteriaField>(ref IQueryable<TEntity> source, CriteriaList<TCriteriaField> criteriaFieldList) where TEntity : class
                                                                                                                                                  where TCriteriaField : CriteriaField
        {
            if (source == null || criteriaFieldList == null)
                return;

            Expression<Func<TEntity, bool>> globalQueryExpression = null, queryExpression;

            foreach (var criteriaField in criteriaFieldList)
            {
                queryExpression = null;
                CompleteQueryInternal(ref queryExpression, criteriaField);
                if (queryExpression != null)
                {
                    if (globalQueryExpression == null)
                        globalQueryExpression = queryExpression;
                    else
                        globalQueryExpression = globalQueryExpression.OrElse(queryExpression);
                }
            }

            if (globalQueryExpression != null)
                source = source.Where(globalQueryExpression);

            foreach (var criteriaField in criteriaFieldList)
            {
                if (criteriaField != null && criteriaField.FieldSortList != null && criteriaField.FieldSortList.Count > 0)
                {
                    ApplySort(ref source, criteriaField);
                    break;
                }
            }
        }

        public static void CompleteQuery<TEntity>(ref IQueryable<TEntity> source, CriteriaField criteriaField) where TEntity : class
        {
            if (source == null || criteriaField == null)
                return;

            Expression<Func<TEntity, bool>> queryExpression = null;
            CompleteQueryInternal(ref queryExpression, criteriaField);
            if (queryExpression != null)
            {
                Expression<Func<TEntity, bool>> queryExpression2 = null;
                CompleteQueryInternal(ref queryExpression2, criteriaField);
                queryExpression = queryExpression.Or(queryExpression2);
            }
            if (queryExpression != null)
                source = source.Where(queryExpression);

            ApplySort(ref source, criteriaField);
        }

        private static void CompleteQueryInternal<TEntity>(ref Expression<Func<TEntity, bool>> queryExpression, CriteriaField criteriaField) where TEntity : class
        {
            var criteriaFieldProperties = criteriaField.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            object value;
            string fieldName;
            var type = typeof(TEntity);
            var properties = type.GetProperties();
            if (properties != null && criteriaFieldProperties != null)
            {
                foreach (var criteriaFieldProperty in criteriaFieldProperties)
                {
                    fieldName = criteriaFieldProperty.Name;
                    value = criteriaFieldProperty.GetValue(criteriaField, null);
                    if (value == null)
                        continue;

                    CompleteQueryWithCriteria(ref queryExpression, fieldName, value);
                }
            }
        }

        private static void CompleteQueryWithCriteria<TEntity>(ref Expression<Func<TEntity, bool>> queryExpression, string fieldName, object criteria) where TEntity : class
        {
            var entityType = typeof(TEntity);
            var entityProperty = entityType.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public);

            if (entityProperty != null)
            {
                var entityParameter = Expression.Parameter(typeof(TEntity), "e");
                var propertyType = entityProperty.PropertyType;

                var expressionList = new List<Expression>();
                Expression expression;

                if (criteria is IntegerCriteria)
                {
                    IntegerCriteriaTreatment(criteria as IntegerCriteria, expressionList, propertyType, entityParameter, entityProperty);
                }
                else if (criteria is StringCriteria)
                {
                    StringCriteriaTreatment(criteria as StringCriteria, expressionList, propertyType, entityParameter, entityProperty);
                }

                expression = StackExpression(expressionList);
                if (expression != null)
                {
                    var lambda = Expression.Lambda(expression, entityParameter) as Expression<Func<TEntity, bool>>;

                    if (queryExpression == null)
                        queryExpression = lambda;
                    else
                        queryExpression = queryExpression.AndAlso(lambda);
                }
            }
        }

        private static Expression StackExpression(List<Expression> expressionList)
        {
            Expression expression = null;
            foreach (var exp in expressionList)
            {
                if (exp == null)
                    continue;

                if (expression == null)
                {
                    expression = exp;
                }
                else
                {
                    expression = Expression.OrElse(expression, exp);
                }
            }
            return expression;
        }

        private static Expression AddEqualExpression<T>(ParameterExpression entityParameter, PropertyInfo entityProperty, T value)
        {
            if (typeof(T) != typeof(string))
            {
                return Expression.Equal(
                        Expression.Property(entityParameter, entityProperty),
                        Expression.Constant(value)
                       );
            }
            return null;
        }

        private static Expression AddEqualStringExpression(ParameterExpression entityParameter, PropertyInfo entityProperty, string value, bool ignoreCase)
        {
            if (ignoreCase)
            {
                var expressionProperty = Expression.Property(entityParameter, entityProperty);
                Expression toLower = Expression.Call(expressionProperty, "ToLower", null, null);
                return Expression.Equal(toLower, Expression.Constant(value != null ? value.ToLowerInvariant() : null));
            }
            else
            {
                var expressionProperty = Expression.Property(entityParameter, entityProperty);
                return Expression.Equal(Expression.Property(entityParameter, entityProperty),
                                        Expression.Constant(value));
            }
        }

        private static Expression AddNotEqualExpression<T>(ParameterExpression entityParameter, PropertyInfo entityProperty, T value)
        {
            if (typeof(T) != typeof(string))
            {
                return Expression.NotEqual(
                        Expression.Property(entityParameter, entityProperty),
                        Expression.Constant(value));
            }
            return null;
        }

        private static Expression AddNotEqualStringExpression(ParameterExpression entityParameter, PropertyInfo entityProperty, string value, bool ignoreCase)
        {
            if (ignoreCase)
            {
                var expressionProperty = Expression.Property(entityParameter, entityProperty);
                Expression toLower = Expression.Call(expressionProperty, "ToLower", null, null);
                return Expression.NotEqual(toLower, Expression.Constant(value != null ? value.ToLowerInvariant() : null));
            }
            else
            {
                var expressionProperty = Expression.Property(entityParameter, entityProperty);
                return Expression.NotEqual(Expression.Property(entityParameter, entityProperty),
                                           Expression.Constant(value));
            }
        }

        private static Expression AddStartsWithStringExpression(ParameterExpression entityParameter, PropertyInfo entityProperty, string value, bool ignoreCase)
        {
            if (ignoreCase)
            {
                var expressionProperty = Expression.Property(entityParameter, entityProperty);
                Expression toLower = Expression.Call(expressionProperty, "ToLower", null, null);
                MethodInfo mi = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
                return Expression.Call(toLower, mi, Expression.Constant(value != null ? value.ToLowerInvariant() : null));
            }
            else
            {
                var expressionProperty = Expression.Property(entityParameter, entityProperty);
                MethodInfo mi = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
                return Expression.Call(expressionProperty, mi, Expression.Constant(value));
            }
        }

        private static Expression AddEndsWithStringExpression(ParameterExpression entityParameter, PropertyInfo entityProperty, string value, bool ignoreCase)
        {
            if (ignoreCase)
            {
                var expressionProperty = Expression.Property(entityParameter, entityProperty);
                Expression toLower = Expression.Call(expressionProperty, "ToLower", null, null);
                MethodInfo mi = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
                return Expression.Call(toLower, mi, Expression.Constant(value != null ? value.ToLowerInvariant() : null));
            }
            else
            {
                var expressionProperty = Expression.Property(entityParameter, entityProperty);
                MethodInfo mi = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
                return Expression.Call(expressionProperty, mi, Expression.Constant(value));
            }
        }

        private static Expression AddContainsStringExpression(ParameterExpression entityParameter, PropertyInfo entityProperty, string value, bool ignoreCase)
        {
            if (ignoreCase)
            {
                var expressionProperty = Expression.Property(entityParameter, entityProperty);
                Expression toLower = Expression.Call(expressionProperty, "ToLower", null, null);
                MethodInfo mi = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
                return Expression.Call(toLower, mi, Expression.Constant(value != null ? value.ToLowerInvariant() : null));
            }
            else
            {
                var expressionProperty = Expression.Property(entityParameter, entityProperty);
                MethodInfo mi = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
                return Expression.Call(expressionProperty, mi, Expression.Constant(value));
            }
        }

        private static void IntegerCriteriaTreatment(IntegerCriteria criteria, List<Expression> expressionList, Type propertyType,
                                                     ParameterExpression entityParameter, PropertyInfo entityProperty)
        {
            if (!propertyType.Equals(typeof(int)))
                return;
            if (criteria.Equal.HasValue)
            {
                expressionList.Add(AddEqualExpression(entityParameter, entityProperty, criteria.Equal.Value));
            }
            if (criteria.EqualList != null)
            {
                foreach (int equalValue in criteria.EqualList)
                {
                    expressionList.Add(AddEqualExpression(entityParameter, entityProperty, equalValue));
                }
            }
            if (criteria.NotEqual.HasValue)
            {
                expressionList.Add(AddNotEqualExpression(entityParameter, entityProperty, criteria.NotEqual.Value));
            }
            if (criteria.NotEqualList != null)
            {
                foreach (int equalValue in criteria.NotEqualList)
                {
                    expressionList.Add(AddNotEqualExpression(entityParameter, entityProperty, equalValue));
                }
            }
        }

        private static void StringCriteriaTreatment(StringCriteria criteria, List<Expression> expressionList, Type propertyType,
                                                    ParameterExpression entityParameter, PropertyInfo entityProperty)
        {
            if (!propertyType.Equals(typeof(string)))
                return;
            if (criteria.Equal != null)
            {
                expressionList.Add(AddEqualStringExpression(entityParameter, entityProperty, criteria.Equal, false));
            }
            if (criteria.EqualList != null)
            {
                foreach (string value in criteria.EqualList)
                {
                    expressionList.Add(AddEqualStringExpression(entityParameter, entityProperty, value, criteria.IgnoreCase));
                }
            }
            if (criteria.NotEqual != null)
            {
                expressionList.Add(AddNotEqualStringExpression(entityParameter, entityProperty, criteria.NotEqual, false));
            }
            if (criteria.NotEqualList != null)
            {
                foreach (string value in criteria.NotEqualList)
                {
                    expressionList.Add(AddNotEqualStringExpression(entityParameter, entityProperty, value, criteria.IgnoreCase));
                }
            }
            if (criteria.StartsWithList != null)
            {
                foreach (string value in criteria.StartsWithList)
                {
                    if (value != null)
                        expressionList.Add(AddStartsWithStringExpression(entityParameter, entityProperty, value, criteria.IgnoreCase));
                }
            }
            if (criteria.EndsWithList != null)
            {
                foreach (string value in criteria.EndsWithList)
                {
                    if (value != null)
                        expressionList.Add(AddEndsWithStringExpression(entityParameter, entityProperty, value, criteria.IgnoreCase));
                }
            }
            if (criteria.ContainsList != null)
            {
                foreach (string value in criteria.ContainsList)
                {
                    if (value != null)
                        expressionList.Add(AddContainsStringExpression(entityParameter, entityProperty, value, criteria.IgnoreCase));
                }
            }
        }

        private static PropertyInfo GetPropertyByName(Type type, string name)
        {
            var criteriaFieldProperties = /*obj.GetType()*/type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var criteriaFieldProperty in criteriaFieldProperties)
            {
                if (criteriaFieldProperty.Name.ToLower() == name.ToLower())
                {

                    return criteriaFieldProperty;
                }
            }
            return null;
        }

        private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            var type = typeof(T);
            var arg = Expression.Parameter(type, "x");
            Expression expr = arg;

            var pi = GetPropertyByName(type, property);
            if (pi != null)
            {
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;

                var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
                var lambda = Expression.Lambda(delegateType, expr, arg);

                var result = typeof(Queryable).GetMethods().Single(
                        method => method.Name == methodName
                                && method.IsGenericMethodDefinition
                                && method.GetGenericArguments().Length == 2
                                && method.GetParameters().Length == 2)
                        .MakeGenericMethod(typeof(T), type)
                        .Invoke(null, new object[] { source, lambda });
                return (IOrderedQueryable<T>)result;
            }
            else
                return null;
        }

        private static void ApplySort<TEntity>(ref IQueryable<TEntity> source, CriteriaField criteriaField) where TEntity : class
        {
            IOrderedQueryable<TEntity> orderedQueryTmp;
            IOrderedQueryable<TEntity> orderedQuery = null;
            if (criteriaField != null && criteriaField.FieldSortList != null && criteriaField.FieldSortList.Count > 0)
            {
                foreach (FieldSort fieldSort in criteriaField.FieldSortList)
                {
                    orderedQueryTmp = null;
                    if (fieldSort.Sort == TFieldSort.Asc)
                        orderedQueryTmp = ApplyOrder(orderedQuery == null ? source : orderedQuery, fieldSort.Name, orderedQuery == null ? "OrderBy" : "ThenBy");
                    else if (fieldSort.Sort == TFieldSort.Desc)
                        orderedQueryTmp = ApplyOrder(orderedQuery == null ? source : orderedQuery, fieldSort.Name, orderedQuery == null ? "OrderByDescending" : "ThenByDescending");

                    if (orderedQueryTmp != null)
                        orderedQuery = orderedQueryTmp;
                }
            }
            if (orderedQuery != null)
                source = orderedQuery;
        }
    }
}
