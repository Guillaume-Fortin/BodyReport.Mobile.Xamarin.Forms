using System;
using SQLite.Net;
using Message;
using BodyReportMobile.Core.Crud.Transformer;
using System.Collections.Generic;
using BodyReportMobile.Core.Models;

namespace BodyReportMobile.Core.Crud.Module
{
	public class TrainingWeekModule : Crud
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContext">database context</param>
		public TrainingWeekModule(SQLiteConnection dbContext) : base(dbContext)
		{
		}

		protected override void CreateTable()
		{
			_dbContext.Execute(@"CREATE TABLE IF NOT EXISTS TrainingWeek (
						  UserId VARCHAR(450) NOT NULL,
						  Year INTEGER NOT NULL,
						  WeekOfYear INTEGER NOT NULL,
						  UserHeight REAL,
						  UserWeight REAL,
						  Unit INTEGER,
						  PRIMARY KEY (UserId, Year, WeekOfYear))");
		}

		/// <summary>
		/// Create data in database
		/// </summary>
		/// <param name="trainingJournal">Data</param>
		/// <returns>insert data</returns>
		public TrainingWeek Create(TrainingWeek trainingJournal)
		{
			if (trainingJournal == null || string.IsNullOrWhiteSpace(trainingJournal.UserId) ||
				trainingJournal.Year == 0 || trainingJournal.WeekOfYear == 0)
				return null;

			var row = new TrainingWeekRow();
			TrainingWeekTransformer.ToRow(trainingJournal, row);
			_dbContext.Insert(row);
			return TrainingWeekTransformer.ToBean(row);
		}

		/// <summary>
		/// Get data in database
		/// </summary>
		/// <param name="key">Primary Key</param>
		/// <returns>read data</returns>
		public TrainingWeek Get(TrainingWeekKey key)
		{
			if (key == null || string.IsNullOrWhiteSpace(key.UserId) ||
				key.Year == 0 || key.WeekOfYear == 0)
				return null;

			var row = _dbContext.Table<TrainingWeekRow>().Where(t => t.UserId == key.UserId && t.Year == key.Year &&
				t.WeekOfYear == key.WeekOfYear).FirstOrDefault();
			if (row != null)
			{
				return TrainingWeekTransformer.ToBean(row);
			}
			return null;
		}

		/// <summary>
		/// Find datas
		/// </summary>
		/// <returns></returns>
		public List<TrainingWeek> Find(CriteriaField criteriaField = null)
		{
			List<TrainingWeek> resultList = null;
			TableQuery<TrainingWeekRow> rowList = _dbContext.Table<TrainingWeekRow>();
			//CriteriaTransformer.CompleteQuery(ref rowList, criteriaField);
			rowList = rowList.OrderBy(t => t.UserId).OrderByDescending(t => t.Year).ThenByDescending(t => t.WeekOfYear);

			if (rowList != null && rowList.Count() > 0)
			{
				resultList = new List<TrainingWeek>();
				foreach (var trainingJournalRow in rowList)
				{
					resultList.Add(TrainingWeekTransformer.ToBean(trainingJournalRow));
				}
			}
			return resultList;
		}

		/// <summary>
		/// Update data in database
		/// </summary>
		/// <param name="trainingJournal">data</param>
		/// <returns>updated data</returns>
		public TrainingWeek Update(TrainingWeek trainingJournal)
		{
			if (trainingJournal == null || string.IsNullOrWhiteSpace(trainingJournal.UserId) ||
				trainingJournal.Year == 0 || trainingJournal.WeekOfYear == 0)
				return null;

			var trainingJournalRow = _dbContext.Table<TrainingWeekRow>().Where(t => t.UserId == trainingJournal.UserId && t.Year == trainingJournal.Year &&
				t.WeekOfYear == trainingJournal.WeekOfYear).FirstOrDefault();
			if (trainingJournalRow == null)
			{ // No data in database
				return Create(trainingJournal);
			}
			else
			{ //Modify Data in database
				TrainingWeekTransformer.ToRow(trainingJournal, trainingJournalRow);
				return TrainingWeekTransformer.ToBean(trainingJournalRow);
			}
		}

		/// <summary>
		/// Delete data in database
		/// </summary>
		/// <param name="key">Primary Key</param>
		public void Delete(TrainingWeekKey key)
		{
			if (key == null || string.IsNullOrWhiteSpace(key.UserId) || key.Year == 0 || key.WeekOfYear == 0)
				return;

			var row = _dbContext.Table<TrainingWeekRow>().Where(t => t.UserId == key.UserId && t.Year == key.Year &&
				t.WeekOfYear == key.WeekOfYear).FirstOrDefault();
			if (row != null)
			{
				_dbContext.Delete(row);
			}
		}
	}
}

