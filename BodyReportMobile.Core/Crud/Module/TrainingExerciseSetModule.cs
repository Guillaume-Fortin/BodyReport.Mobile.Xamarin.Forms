﻿using System;
using BodyReport.Message;
using BodyReportMobile.Core.Crud.Transformer;
using System.Collections.Generic;
using BodyReportMobile.Core.Models;
using BodyReportMobile.Core.Data;
using System.Linq;

namespace BodyReportMobile.Core.Crud.Module
{
	public class TrainingExerciseSetModule : Crud
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContext">database context</param>
		public TrainingExerciseSetModule(ApplicationDbContext dbContext) : base(dbContext)
		{
		}

		/// <summary>
		/// Create data in database
		/// </summary>
		/// <param name="trainingExerciseSet">Data</param>
		/// <returns>insert data</returns>
		public TrainingExerciseSet Create(TrainingExerciseSet trainingExerciseSet)
		{
			if (trainingExerciseSet == null || string.IsNullOrWhiteSpace(trainingExerciseSet.UserId) ||
				trainingExerciseSet.Year == 0 || trainingExerciseSet.WeekOfYear == 0 ||
				trainingExerciseSet.DayOfWeek < 0 || trainingExerciseSet.DayOfWeek > 6 || trainingExerciseSet.TrainingDayId == 0 ||
				trainingExerciseSet.TrainingExerciseId == 0 || trainingExerciseSet.Id == 0)
				return null;

			var row = new TrainingExerciseSetRow();
			TrainingExerciseSetTransformer.ToRow(trainingExerciseSet, row);
			_dbContext.TrainingExerciseSet.Add(row);
            _dbContext.SaveChanges();
            return TrainingExerciseSetTransformer.ToBean(row);
		}

		/// <summary>
		/// Get data in database
		/// </summary>
		/// <param name="key">Primary Key</param>
		/// <returns>read data</returns>
		public TrainingExerciseSet Get(TrainingExerciseSetKey key)
		{
			if (key == null || string.IsNullOrWhiteSpace(key.UserId) ||
				key.Year == 0 || key.WeekOfYear == 0 || key.DayOfWeek < 0 || key.DayOfWeek > 6 || key.TrainingExerciseId == 0 || key.Id == 0)
				return null;

			var row = _dbContext.TrainingExerciseSet.Where(t => t.UserId == key.UserId && t.Year == key.Year &&
				t.WeekOfYear == key.WeekOfYear && t.DayOfWeek == key.DayOfWeek &&
				t.TrainingDayId == key.TrainingDayId && 
				t.TrainingExerciseId == key.TrainingExerciseId && t.Id == key.Id).FirstOrDefault();
			if (row != null)
			{
				return TrainingExerciseSetTransformer.ToBean(row);
			}
			return null;
		}

		/// <summary>
		/// Find datas
		/// </summary>
		/// <returns></returns>
		public List<TrainingExerciseSet> Find(TrainingExerciseSetCriteria trainingExerciseSetCriteria = null)
		{
			List<TrainingExerciseSet> resultList = null;
            IQueryable<TrainingExerciseSetRow> rowList = _dbContext.TrainingExerciseSet;
			CriteriaTransformer.CompleteQuery(ref rowList, trainingExerciseSetCriteria);
			rowList = rowList.OrderBy(t => t.TrainingExerciseId);

			if (rowList != null)
			{
				foreach (var row in rowList)
				{
                    if (resultList == null)
                        resultList = new List<TrainingExerciseSet>();
                    resultList.Add(TrainingExerciseSetTransformer.ToBean(row));
				}
			}
			return resultList;
		}

        /// <summary>
		/// Find datas
		/// </summary>
		/// <returns></returns>
		public List<TrainingExerciseSet> Find(CriteriaList<TrainingExerciseSetCriteria> trainingExerciseSetCriteriaList = null)
        {
            List<TrainingExerciseSet> resultList = null;
            IQueryable<TrainingExerciseSetRow> rowList = _dbContext.TrainingExerciseSet;
            CriteriaTransformer.CompleteQuery(ref rowList, trainingExerciseSetCriteriaList);
            rowList = rowList.OrderBy(t => t.TrainingExerciseId);

            if (rowList != null)
            {
                foreach (var row in rowList)
                {
                    if (resultList == null)
                        resultList = new List<TrainingExerciseSet>();
                    resultList.Add(TrainingExerciseSetTransformer.ToBean(row));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Update data in database
        /// </summary>
        /// <param name="trainingExerciseSet">data</param>
        /// <returns>updated data</returns>
        public TrainingExerciseSet Update(TrainingExerciseSet trainingExerciseSet)
		{
			if (trainingExerciseSet == null || string.IsNullOrWhiteSpace(trainingExerciseSet.UserId) ||
				trainingExerciseSet.Year == 0 || trainingExerciseSet.WeekOfYear == 0 ||
				trainingExerciseSet.DayOfWeek < 0 || trainingExerciseSet.DayOfWeek > 6 || trainingExerciseSet.TrainingDayId == 0 ||
				trainingExerciseSet.TrainingExerciseId == 0 || trainingExerciseSet.Id == 0)
				return null;

			var row = _dbContext.TrainingExerciseSet.Where(t => t.UserId == trainingExerciseSet.UserId &&
				t.Year == trainingExerciseSet.Year &&
				t.WeekOfYear == trainingExerciseSet.WeekOfYear &&
				t.DayOfWeek == trainingExerciseSet.DayOfWeek &&
				t.TrainingDayId == trainingExerciseSet.TrainingDayId &&
				t.TrainingExerciseId == trainingExerciseSet.TrainingExerciseId &&
				t.Id == trainingExerciseSet.Id).FirstOrDefault();
			if (row == null)
			{ // No data in database
				return Create(trainingExerciseSet);
			}
			else
			{ //Modify Data in database
				TrainingExerciseSetTransformer.ToRow(trainingExerciseSet, row);
                _dbContext.SaveChanges();
                return TrainingExerciseSetTransformer.ToBean(row);
			}
		}

		/// <summary>
		/// Delete data in database
		/// </summary>
		/// <param name="key">Primary Key</param>
		public void Delete(TrainingExerciseSetKey key)
		{
			if (key == null || string.IsNullOrWhiteSpace(key.UserId) || key.Year == 0 || key.WeekOfYear == 0 ||
				key.DayOfWeek < 0 || key.DayOfWeek > 6 || key.TrainingExerciseId == 0 || key.Id == 0)
				return;

			var row = _dbContext.TrainingExerciseSet.Where(t => t.UserId == key.UserId && t.Year == key.Year &&
				t.WeekOfYear == key.WeekOfYear && t.DayOfWeek == key.DayOfWeek &&
				t.TrainingDayId == key.TrainingDayId && t.TrainingExerciseId == key.TrainingExerciseId &&
				t.Id == key.Id).FirstOrDefault();
			if (row != null)
			{
				_dbContext.TrainingExerciseSet.Remove(row);
                _dbContext.SaveChanges();
            }
		}
	}
}

