using System;
using SQLite.Net;
using BodyReport.Message;
using System.Collections.Generic;
using BodyReportMobile.Core.Crud.Module;
using BodyReportMobile.Core.ServiceLayers;

namespace BodyReportMobile.Core.Manager
{
	public class TrainingWeekManager : BodyReportManager
    {
		TrainingWeekModule _trainingWeekModule = null;

        TrainingDayService _trainingDayService = null;

        public TrainingWeekManager(SQLiteConnection dbContext) : base(dbContext)
		{
			_trainingWeekModule = new TrainingWeekModule(DbContext);
            _trainingDayService = new TrainingDayService(DbContext);
        }

		internal TrainingWeek CreateTrainingWeek(TrainingWeek trainingWeek)
		{
			TrainingWeek trainingWeekResult = null;
			trainingWeekResult = _trainingWeekModule.Create(trainingWeek);

			if (trainingWeek.TrainingDays != null)
			{
				trainingWeekResult.TrainingDays = new List<TrainingDay>();
				foreach (var trainingDay in trainingWeek.TrainingDays)
				{
                    trainingWeekResult.TrainingDays.Add(_trainingDayService.CreateTrainingDay(trainingDay));
				}
			}
			return trainingWeekResult;
		}

		internal TrainingWeek UpdateTrainingWeek(TrainingWeek trainingWeek, TrainingWeekScenario trainingWeekScenario)
		{
			TrainingWeek trainingWeekResult = null;
			trainingWeekResult = _trainingWeekModule.Update(trainingWeek);

			if (trainingWeekScenario!= null && trainingWeekScenario.ManageTrainingDay)
			{
                var trainingDayCriteria = new TrainingDayCriteria()
                {
                    UserId = new StringCriteria() { Equal = trainingWeek.UserId },
                    Year = new IntegerCriteria() { Equal = trainingWeek.Year },
                    WeekOfYear = new IntegerCriteria() { Equal = trainingWeek.WeekOfYear }
                };
                var trainingDaysDb = _trainingDayService.FindTrainingDay(trainingDayCriteria, trainingWeekScenario.TrainingDayScenario);
                if (trainingDaysDb != null && trainingDaysDb.Count > 0)
                {
                    foreach (var trainingDayDb in trainingDaysDb)
                        _trainingDayService.DeleteTrainingDay(trainingDayDb);
                }

                if (trainingWeek.TrainingDays != null)
                {
                    trainingWeekResult.TrainingDays = new List<TrainingDay>();
                    foreach (var trainingDay in trainingWeek.TrainingDays)
                    {
                        trainingWeekResult.TrainingDays.Add(_trainingDayService.UpdateTrainingDay(trainingDay, trainingWeekScenario.TrainingDayScenario));
                    }
                }
			}
			return trainingWeekResult;
		}

		internal TrainingWeek GetTrainingWeek(TrainingWeekKey key, TrainingWeekScenario trainingWeekScenario)
		{
			var trainingWeek = _trainingWeekModule.Get(key);
			if (trainingWeek != null && trainingWeekScenario != null && trainingWeekScenario.ManageTrainingDay)
			{
				CompleteTrainingWeekWithTrainingDay(trainingWeek, trainingWeekScenario.TrainingDayScenario);
			}

			return trainingWeek;
		}

		private void CompleteTrainingWeekWithTrainingDay(TrainingWeek trainingWeek, TrainingDayScenario trainingDayScenario)
		{
			if (trainingWeek != null)
			{
				var trainingDayCriteria = new TrainingDayCriteria()
				{
					UserId = new StringCriteria() { Equal = trainingWeek.UserId },
					Year = new IntegerCriteria() { Equal = trainingWeek.Year },
					WeekOfYear = new IntegerCriteria() { Equal = trainingWeek.WeekOfYear },
				};
				trainingWeek.TrainingDays = _trainingDayService.FindTrainingDay(trainingDayCriteria, trainingDayScenario);
			}
		}

		public List<TrainingWeek> FindTrainingWeek(TrainingWeekCriteria trainingWeekCriteria, TrainingWeekScenario trainingWeekScenario)
		{
			List<TrainingWeek> trainingWeeks = _trainingWeekModule.Find(trainingWeekCriteria);

			if (trainingWeeks != null && trainingWeekScenario != null && trainingWeekScenario.TrainingDayScenario != null)
			{
				foreach (TrainingWeek trainingJournal in trainingWeeks)
				{
					CompleteTrainingWeekWithTrainingDay(trainingJournal, trainingWeekScenario.TrainingDayScenario);
				}
			}

			return trainingWeeks;
		}

		internal void DeleteTrainingWeek(TrainingWeekKey key)
		{
            var trainingWeekScenario = new TrainingWeekScenario()
            {
                ManageTrainingDay = true,
                TrainingDayScenario = new TrainingDayScenario() { ManageExercise = true }
            };
            var trainingWeek = GetTrainingWeek(key, trainingWeekScenario);
			if (trainingWeek != null)
			{
				_trainingWeekModule.Delete(key);

				if (trainingWeek.TrainingDays != null)
				{
					foreach (var trainingDay in trainingWeek.TrainingDays)
					{
                        _trainingDayService.DeleteTrainingDay(trainingDay);
					}
				}
			}
		}
	}
}

