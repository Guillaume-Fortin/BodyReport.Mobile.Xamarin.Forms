using System;
using SQLite.Net;
using Message;
using BodyReportMobile.Core.Crud.Transformer;
using System.Collections.Generic;
using BodyReportMobile.Core.Models;

namespace BodyReportMobile.Core.Crud.Module
{
	public class TrainingDayModule : Crud
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContext">database context</param>
		public TrainingDayModule(SQLiteConnection dbContext) : base(dbContext)
		{
		}

		protected override void CreateTable()
		{
			_dbContext.Execute(@"CREATE TABLE IF NOT EXISTS TrainingDay (
						  UserId VARCHAR(450) NOT NULL,
						  Year INTEGER NOT NULL,
						  WeekOfYear INTEGER NOT NULL,
						  DayOfWeek INTEGER NOT NULL,
						  TrainingDayId INTEGER NOT NULL,
						  BeginHour NUMERIC,
						  EndHour NUMERIC,
						  PRIMARY KEY (UserId, Year, WeekOfYear, DayOfWeek, TrainingDayId))");
		}

		/// <summary>
		/// Create data in database
		/// </summary>
		/// <param name="trainingJournalDay">Data</param>
		/// <returns>insert data</returns>
		public TrainingDay Create(TrainingDay trainingJournalDay)
		{
			if (trainingJournalDay == null || string.IsNullOrWhiteSpace(trainingJournalDay.UserId) ||
				trainingJournalDay.Year == 0 || trainingJournalDay.WeekOfYear == 0 ||
				trainingJournalDay.DayOfWeek < 0 || trainingJournalDay.DayOfWeek > 6 || trainingJournalDay.TrainingDayId == 0)
				return null;

			var row = new TrainingDayRow();
			TrainingDayTransformer.ToRow(trainingJournalDay, row);
			_dbContext.Insert(row);
			return TrainingDayTransformer.ToBean(row);
		}

		/// <summary>
		/// Get data in database
		/// </summary>
		/// <param name="key">Primary Key</param>
		/// <returns>read data</returns>
		public TrainingDay Get(TrainingDayKey key)
		{
			if (key == null || string.IsNullOrWhiteSpace(key.UserId) ||
				key.Year == 0 || key.WeekOfYear == 0 || key.DayOfWeek < 0 || key.DayOfWeek > 6 || key.TrainingDayId == 0)
				return null;

			var row = _dbContext.Table<TrainingDayRow>().Where(t => t.UserId == key.UserId &&
				t.Year == key.Year &&
				t.WeekOfYear == key.WeekOfYear &&
				t.DayOfWeek == key.DayOfWeek &&
				t.TrainingDayId == key.TrainingDayId).FirstOrDefault();
			if (row != null)
			{
				return TrainingDayTransformer.ToBean(row);
			}
			return null;
		}

		/// <summary>
		/// Find datas
		/// </summary>
		/// <returns></returns>
		public List<TrainingDay> Find(CriteriaField criteriaField = null)
		{
			List<TrainingDay> resultList = null;
			TableQuery<TrainingDayRow> rowList = _dbContext.Table<TrainingDayRow>();
			//CriteriaTransformer.CompleteQuery(ref rowList, criteriaField);
			rowList = rowList.OrderBy(t => t.DayOfWeek).OrderBy(t => t.BeginHour);

			if (rowList != null && rowList.Count() > 0)
			{
				resultList = new List<TrainingDay>();
				foreach (var trainingJournalDayRow in rowList)
				{
					resultList.Add(TrainingDayTransformer.ToBean(trainingJournalDayRow));
				}
			}
			return resultList;
		}

		/// <summary>
		/// Update data in database
		/// </summary>
		/// <param name="trainingJournalDay">data</param>
		/// <returns>updated data</returns>
		public TrainingDay Update(TrainingDay trainingJournalDay)
		{
			if (trainingJournalDay == null || string.IsNullOrWhiteSpace(trainingJournalDay.UserId) ||
				trainingJournalDay.Year == 0 || trainingJournalDay.WeekOfYear == 0 ||
				trainingJournalDay.DayOfWeek < 0 || trainingJournalDay.DayOfWeek > 6 || trainingJournalDay.TrainingDayId == 0)
				return null;

			var trainingJournalRow = _dbContext.Table<TrainingDayRow>().Where(t=>t.UserId == trainingJournalDay.UserId &&
				t.Year == trainingJournalDay.Year &&
				t.WeekOfYear == trainingJournalDay.WeekOfYear &&
				t.DayOfWeek == trainingJournalDay.DayOfWeek &&
				t.TrainingDayId == trainingJournalDay.TrainingDayId).FirstOrDefault();
			if (trainingJournalRow == null)
			{ // No data in database
				return Create(trainingJournalDay);
			}
			else
			{ //Modify Data in database
				TrainingDayTransformer.ToRow(trainingJournalDay, trainingJournalRow);
				return TrainingDayTransformer.ToBean(trainingJournalRow);
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

			var row = _dbContext.Table<TrainingDayRow>().Where(t => t.UserId == key.UserId && t.Year == key.Year &&
				t.WeekOfYear == key.WeekOfYear && t.DayOfWeek == key.DayOfWeek &&
				t.TrainingDayId == key.TrainingDayId).FirstOrDefault();
			if (row != null)
			{
				_dbContext.Delete(row);
			}
		}
	}
}

