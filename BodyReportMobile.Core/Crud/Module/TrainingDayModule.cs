using System;
using BodyReport.Message;
using BodyReportMobile.Core.Crud.Transformer;
using System.Collections.Generic;
using BodyReportMobile.Core.Models;
using BodyReportMobile.Core.Data;
using System.Linq;

namespace BodyReportMobile.Core.Crud.Module
{
	public class TrainingDayModule : Crud
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContext">database context</param>
		public TrainingDayModule(ApplicationDbContext dbContext) : base(dbContext)
		{
		}

		/// <summary>
		/// Create data in database
		/// </summary>
		/// <param name="trainingDay">Data</param>
		/// <returns>insert data</returns>
		public TrainingDay Create(TrainingDay trainingDay, TUnitType userUnit)
		{
			if (trainingDay == null || string.IsNullOrWhiteSpace(trainingDay.UserId) ||
				trainingDay.Year == 0 || trainingDay.WeekOfYear == 0 ||
				trainingDay.DayOfWeek < 0 || trainingDay.DayOfWeek > 6 || trainingDay.TrainingDayId == 0)
				return null;

			var row = new TrainingDayRow();
			TrainingDayTransformer.ToRow(trainingDay, row);
			_dbContext.TrainingDay.Add(row);
            _dbContext.SaveChanges();
            return TrainingDayTransformer.ToBean(row, userUnit);
		}

		/// <summary>
		/// Get data in database
		/// </summary>
		/// <param name="key">Primary Key</param>
		/// <returns>read data</returns>
		public TrainingDay Get(TrainingDayKey key, TUnitType userUnit)
		{
			if (key == null || string.IsNullOrWhiteSpace(key.UserId) ||
				key.Year == 0 || key.WeekOfYear == 0 || key.DayOfWeek < 0 || key.DayOfWeek > 6 || key.TrainingDayId == 0)
				return null;

			var row = _dbContext.TrainingDay.Where(t => t.UserId == key.UserId &&
				t.Year == key.Year &&
				t.WeekOfYear == key.WeekOfYear &&
				t.DayOfWeek == key.DayOfWeek &&
				t.TrainingDayId == key.TrainingDayId).FirstOrDefault();
			if (row != null)
			{
				return TrainingDayTransformer.ToBean(row, userUnit);
			}
			return null;
		}

		/// <summary>
		/// Find datas
		/// </summary>
		/// <returns></returns>
		public List<TrainingDay> Find(TUnitType userUnit, TrainingDayCriteria trainingDayCriteria = null)
		{
			List<TrainingDay> resultList = null;
            IQueryable<TrainingDayRow> rowList = _dbContext.TrainingDay;
			CriteriaTransformer.CompleteQuery(ref rowList, trainingDayCriteria);
			rowList = rowList.OrderBy(t => t.DayOfWeek).OrderBy(t => t.BeginHour);

			if (rowList != null)
			{
				foreach (var trainingJournalDayRow in rowList)
				{
                    if (resultList == null)
                        resultList = new List<TrainingDay>();
                    resultList.Add(TrainingDayTransformer.ToBean(trainingJournalDayRow, userUnit));
				}
			}
			return resultList;
		}

		/// <summary>
		/// Update data in database
		/// </summary>
		/// <param name="trainingDay">data</param>
		/// <returns>updated data</returns>
		public TrainingDay Update(TrainingDay trainingDay, TUnitType userUnit)
		{
			if (trainingDay == null || string.IsNullOrWhiteSpace(trainingDay.UserId) ||
				trainingDay.Year == 0 || trainingDay.WeekOfYear == 0 ||
				trainingDay.DayOfWeek < 0 || trainingDay.DayOfWeek > 6 || trainingDay.TrainingDayId == 0)
				return null;

			var trainingJournalRow = _dbContext.TrainingDay.Where(t=>t.UserId == trainingDay.UserId &&
				t.Year == trainingDay.Year &&
				t.WeekOfYear == trainingDay.WeekOfYear &&
				t.DayOfWeek == trainingDay.DayOfWeek &&
				t.TrainingDayId == trainingDay.TrainingDayId).FirstOrDefault();
			if (trainingJournalRow == null)
			{ // No data in database
				return Create(trainingDay, userUnit);
			}
			else
			{ //Modify Data in database
				TrainingDayTransformer.ToRow(trainingDay, trainingJournalRow);
                _dbContext.SaveChanges();
                return TrainingDayTransformer.ToBean(trainingJournalRow, userUnit);
			}
		}

		/// <summary>
		/// Delete data in database
		/// </summary>
		/// <param name="key">Primary Key</param>
		public void Delete(TrainingDayKey key)
		{
			if (key == null || string.IsNullOrWhiteSpace(key.UserId) || key.Year == 0 || 
				key.WeekOfYear == 0 || key.DayOfWeek < 0 || key.DayOfWeek > 6 || key.TrainingDayId == 0)
				return;

			var row = _dbContext.TrainingDay.Where(t => t.UserId == key.UserId && t.Year == key.Year &&
				t.WeekOfYear == key.WeekOfYear && t.DayOfWeek == key.DayOfWeek &&
				t.TrainingDayId == key.TrainingDayId).FirstOrDefault();
			if (row != null)
			{
				_dbContext.TrainingDay.Remove(row);
                _dbContext.SaveChanges();
            }
		}
	}
}

