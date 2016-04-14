using System;
using BodyReportMobile.Core.ServiceManagers;
using SQLite.Net;
using Message;
using System.Collections.Generic;
using BodyReportMobile.Core.Crud.Module;

namespace BodyReportMobile.Core.ServiceManagers
{
	public class TrainingWeekManager : ServiceManager
	{
		TrainingWeekModule _trainingWeekModule = null;
		public TrainingWeekManager(SQLiteConnection dbContext) : base(dbContext)
		{
			_trainingWeekModule = new TrainingWeekModule(_dbContext);
		}

		internal TrainingWeek CreateTrainingWeek(TrainingWeek trainingWeek)
		{
			TrainingWeek trainingWeekResult = null;
			trainingWeekResult = _trainingWeekModule.Create(trainingWeek);

			if (trainingWeek.TrainingDays != null)
			{
				var trainingDayManager = new TrainingDayManager(_dbContext);
				trainingWeekResult.TrainingDays = new List<TrainingDay>();
				foreach (var trainingDay in trainingWeek.TrainingDays)
				{
					trainingWeekResult.TrainingDays.Add(trainingDayManager.CreateTrainingDay(trainingDay));
				}
			}
			return trainingWeekResult;
		}

		internal TrainingWeek UpdateTrainingWeek(TrainingWeek trainingWeek)
		{
			TrainingWeek trainingWeekResult = null;
			trainingWeekResult = _trainingWeekModule.Update(trainingWeek);

			if (trainingWeek.TrainingDays != null)
			{
				var trainingDayManager = new TrainingDayManager(_dbContext);
				trainingWeekResult.TrainingDays = new List<TrainingDay>();
				foreach (var trainingDay in trainingWeek.TrainingDays)
				{
					trainingWeekResult.TrainingDays.Add(trainingDayManager.UpdateTrainingDay(trainingDay));
				}
			}
			return trainingWeekResult;
		}

		internal TrainingWeek GetTrainingWeek(TrainingWeekKey key, bool manageTrainingDay)
		{
			var trainingWeek = _trainingWeekModule.Get(key);
			if (trainingWeek != null && manageTrainingDay)
			{
				CompleteTrainingWeekWithTrainingDay(trainingWeek);
			}

			return trainingWeek;
		}

		private void CompleteTrainingWeekWithTrainingDay(TrainingWeek trainingWeek)
		{
			if (trainingWeek != null)
			{
				var trainingDayManager = new TrainingDayManager(_dbContext);
				var trainingDayCriteria = new TrainingDayCriteria()
				{
					UserId = new StringCriteria() { EqualList = new List<string>() { trainingWeek.UserId } },
					Year = new IntegerCriteria() { EqualList = new List<int>() { trainingWeek.Year } },
					WeekOfYear = new IntegerCriteria() { EqualList = new List<int>() { trainingWeek.WeekOfYear } },
				};
				trainingWeek.TrainingDays = trainingDayManager.FindTrainingDay(trainingDayCriteria, true);
			}
		}

		public List<TrainingWeek> FindTrainingWeek(CriteriaField criteriaField, bool manageTrainingDay)
		{
			List<TrainingWeek> trainingWeeks = _trainingWeekModule.Find(criteriaField);

			if (manageTrainingDay && trainingWeeks != null)
			{
				foreach (TrainingWeek trainingJournal in trainingWeeks)
				{
					CompleteTrainingWeekWithTrainingDay(trainingJournal);
				}
			}

			return trainingWeeks;
		}

		internal void DeleteTrainingWeek(TrainingWeekKey key)
		{
			//TODO manage training Day and TrainingExercise
			var trainingWeek = GetTrainingWeek(key, true);
			if (trainingWeek != null)
			{
				_trainingWeekModule.Delete(key);

				if (trainingWeek.TrainingDays != null)
				{
					var trainingDayManager = new TrainingDayManager(_dbContext);
					foreach (var trainingDay in trainingWeek.TrainingDays)
					{
						trainingDayManager.DeleteTrainingDay(trainingDay);
					}
				}
			}
		}
	}
}

