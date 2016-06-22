using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;
using SQLite.Net;
using BodyReport.Framework;

namespace BodyReportMobile.Core.Crud.Transformer
{
    public static class CriteriaTransformer
    {
        //take on http://stackoverflow.com/questions/7754018/how-to-build-case-insensitive-strong-typed-linq-query-in-c
        private static MethodInfo miToLower = typeof(String).GetRuntimeMethod("ToLower", new Type[] { });
        private static MethodInfo miStartsWith = typeof(String).GetRuntimeMethod("StartsWith", new Type[] { typeof(String) });
        private static MethodInfo miContains = typeof(String).GetRuntimeMethod("Contains", new Type[] { typeof(String) });
        private static MethodInfo miEndsWith = typeof(String).GetRuntimeMethod("EndsWith", new Type[] { typeof(String) });
        
        static CriteriaTransformer()
        {
        }

        public static void CompleteQuery<TEntity, T>(ref IQueryable<TEntity> source, CriteriaList<T> criteriaFieldList) where TEntity : class
                                                                                                                         where T : CriteriaField
        {
            if (criteriaFieldList == null)
                return;

            Expression<Func<TEntity, bool>> queryExpression, globalQueryExpression;
            globalQueryExpression = null;
            foreach (T criteriaField in criteriaFieldList)
            {
                if (criteriaField != null)
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
            }
            if (globalQueryExpression != null)
                source = source.Where(globalQueryExpression);
        }

        public static void CompleteQuery<TEntity>(ref TableQuery<TEntity> source, CriteriaField criteriaField) where TEntity : class
        {
            if (source == null || criteriaField == null)
                return;

            Expression<Func<TEntity, bool>> queryExpression = null;
            CompleteQueryInternal(ref queryExpression, criteriaField);
            if (queryExpression != null)
                source = source.Where(queryExpression);
        }

        private static void CompleteQueryInternal<TEntity>(ref Expression<Func<TEntity, bool>> queryExpression, CriteriaField criteriaField) where TEntity : class
        {
            var criteriaFieldProperties = criteriaField.GetType().GetRuntimeProperties();

            object value;
            string fieldName;
            var type = typeof(TEntity);
            var properties = type.GetRuntimeProperties();
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
            var entityProperty = entityType.GetRuntimeProperty(fieldName);

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
            if (!(value is string))
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
            MemberExpression m = Expression.MakeMemberAccess(entityParameter, entityProperty);

            if (value == null)
            {
                return Expression.Equal(m, Expression.Constant(null));
            }
            else
            {
                var toLower = Expression.Call(m, miToLower);
                return Expression.Equal(toLower, Expression.Constant((value as string).ToLower()));
            }
        }

        private static Expression AddNotEqualExpression<T>(ParameterExpression entityParameter, PropertyInfo entityProperty, T value)
        {
            if (typeof(T) != typeof(string))
            {
                return Expression.NotEqual(
                        Expression.Property(entityParameter, entityProperty),
                        Expression.Constant(value)
                       );
            }
            return null;
        }

        private static Expression AddNotEqualStringExpression(ParameterExpression entityParameter, PropertyInfo entityProperty, string value, bool ignoreCase)
        {
            MemberExpression m = Expression.MakeMemberAccess(entityParameter, entityProperty);
            if (value == null)
            {
                return Expression.NotEqual(m, Expression.Constant(null));
            }
            else
            {
                var toLower = Expression.Call(m, miToLower);
                return Expression.NotEqual(toLower, Expression.Constant((value as string).ToLower()));
            }
        }

        private static Expression AddStartsWithStringExpression(ParameterExpression entityParameter, PropertyInfo entityProperty, string value, bool ignoreCase)
        {
            if (value == null)
                return null;
            else
            {
                MemberExpression m = Expression.MakeMemberAccess(entityParameter, entityProperty);
                var startWith = Expression.Call(m, miStartsWith);
                return Expression.Equal(startWith, Expression.Constant((value as string).ToLower()));
            }
        }

        private static Expression AddEndsWithStringExpression(ParameterExpression entityParameter, PropertyInfo entityProperty, string value, bool ignoreCase)
        {
            if (value == null)
                return null;
            else
            {
                MemberExpression m = Expression.MakeMemberAccess(entityParameter, entityProperty);
                var endWith = Expression.Call(m, miEndsWith);
                return Expression.Equal(endWith, Expression.Constant((value as string).ToLower()));
            }
        }

        private static Expression AddContainsStringExpression(ParameterExpression entityParameter, PropertyInfo entityProperty, string value, bool ignoreCase)
        {
            if (value == null)
                return null;
            else
            {
                MemberExpression m = Expression.MakeMemberAccess(entityParameter, entityProperty);
                var contains = Expression.Call(m, miContains);
                return Expression.Equal(contains, Expression.Constant((value as string).ToLower()));
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
                foreach (int value in criteria.EqualList)
                {
                    expressionList.Add(AddEqualExpression(entityParameter, entityProperty, value));
                }
            }
            if (criteria.NotEqual.HasValue)
            {
                expressionList.Add(AddNotEqualExpression(entityParameter, entityProperty, criteria.NotEqual.Value));
            }
            if (criteria.NotEqualList != null)
            {
                foreach (int value in criteria.NotEqualList)
                {
                    expressionList.Add(AddNotEqualExpression(entityParameter, entityProperty, value));
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
                foreach (string value in criteria.NotEqualList)
                {
                    if (value != null)
                        expressionList.Add(AddEndsWithStringExpression(entityParameter, entityProperty, value, criteria.IgnoreCase));
                }
            }
            if (criteria.ContainsList != null)
            {
                foreach (string value in criteria.NotEqualList)
                {
                    if(value != null)
                        expressionList.Add(AddContainsStringExpression(entityParameter, entityProperty, value, criteria.IgnoreCase));
                }
            }
        }
    }
}
