using System;
using BodyReport.Message;
using BodyReportMobile.Core.Crud.Transformer;
using System.Collections.Generic;
using BodyReportMobile.Core.Models;
using BodyReportMobile.Core.Data;
using System.Linq;

namespace BodyReportMobile.Core.Crud.Module
{
	public class TrainingWeekModule : Crud
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContext">database context</param>
		public TrainingWeekModule(ApplicationDbContext dbContext) : base(dbContext)
		{
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
			_dbContext.TrainingWeek.Add(row);
            _dbContext.SaveChanges();
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

			var row = _dbContext.TrainingWeek.Where(t => t.UserId == key.UserId && t.Year == key.Year &&
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
		public List<TrainingWeek> Find(TrainingWeekCriteria trainingWeekCriteria = null)
		{
			List<TrainingWeek> resultList = null;
			IQueryable<TrainingWeekRow> rowList = _dbContext.TrainingWeek;
			CriteriaTransformer.CompleteQuery(ref rowList, trainingWeekCriteria);
			rowList = rowList.OrderBy(t => t.UserId).OrderByDescending(t => t.Year).ThenByDescending(t => t.WeekOfYear);

			if (rowList != null)
			{
				foreach (var trainingJournalRow in rowList)
				{
                    if (resultList == null)
                        resultList = new List<TrainingWeek>();
                    resultList.Add(TrainingWeekTransformer.ToBean(trainingJournalRow));
				}
			}
			return resultList;
		}

		/// <summary>
		/// Update data in database
		/// </summary>
		/// <param name="trainingWeek">data</param>
		/// <returns>updated data</returns>
		public TrainingWeek Update(TrainingWeek trainingWeek)
		{
			if (trainingWeek == null || string.IsNullOrWhiteSpace(trainingWeek.UserId) ||
				trainingWeek.Year == 0 || trainingWeek.WeekOfYear == 0)
				return null;

			var trainingWeekRow = _dbContext.TrainingWeek.Where(t => t.UserId == trainingWeek.UserId && t.Year == trainingWeek.Year &&
				t.WeekOfYear == trainingWeek.WeekOfYear).FirstOrDefault();
			if (trainingWeekRow == null)
			{ // No data in database
				return Create(trainingWeek);
			}
			else
			{ //Modify Data in database
				TrainingWeekTransformer.ToRow(trainingWeek, trainingWeekRow);
                _dbContext.SaveChanges();
                return TrainingWeekTransformer.ToBean(trainingWeekRow);
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

			var row = _dbContext.TrainingWeek.Where(t => t.UserId == key.UserId && t.Year == key.Year &&
				t.WeekOfYear == key.WeekOfYear).FirstOrDefault();
			if (row != null)
			{
				_dbContext.TrainingWeek.Remove(row);
                _dbContext.SaveChanges();
            }
		}
	}
}

