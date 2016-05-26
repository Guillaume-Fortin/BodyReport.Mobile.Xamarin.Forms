﻿using System;
using SQLite.Net;
using Message;
using System.Collections.Generic;
using BodyReportMobile.Core.Crud.Module;

namespace BodyReportMobile.Core.ServiceManagers
{
	public class TrainingDayManager : ServiceManager
	{
		TrainingDayModule _trainingDayModule = null;

		public TrainingDayManager(SQLiteConnection dbContext) : base(dbContext)
		{
			_trainingDayModule = new TrainingDayModule(_dbContext);
		}

		internal TrainingDay CreateTrainingDay(TrainingDay trainingDay)
		{
			TrainingDay trainingDayResult = null;
			trainingDayResult = _trainingDayModule.Create(trainingDay);

			if (trainingDay.TrainingExercises != null)
			{
				var trainingExerciseManager = new TrainingExerciseManager(_dbContext);
				trainingDayResult.TrainingExercises = new List<TrainingExercise>();
				foreach (var trainingExercise in trainingDay.TrainingExercises)
				{
					trainingDayResult.TrainingExercises.Add(trainingExerciseManager.CreateTrainingExercise(trainingExercise));
				}
			}

			return trainingDayResult;
		}

		private void CompleteTrainingDayWithExercise(TrainingDay trainingJournalDay)
		{
			if (trainingJournalDay != null)
			{
				var trainingExerciseCriteria = new TrainingExerciseCriteria()
				{
					UserId = new StringCriteria() { Equal = trainingJournalDay.UserId},
					Year = new IntegerCriteria() { Equal = trainingJournalDay.Year},
					WeekOfYear = new IntegerCriteria() { Equal = trainingJournalDay.WeekOfYear },
					DayOfWeek = new IntegerCriteria() { Equal = trainingJournalDay.DayOfWeek },
					TrainingDayId = new IntegerCriteria() { Equal = trainingJournalDay.TrainingDayId }
				};
				var trainingExerciseManager = new TrainingExerciseManager(_dbContext);
				trainingJournalDay.TrainingExercises = trainingExerciseManager.FindTrainingExercise(trainingExerciseCriteria);
			}
		}

		internal TrainingDay GetTrainingDay(TrainingDayKey key, bool manageExercise)
		{
			var trainingDay = _trainingDayModule.Get(key);

			if (manageExercise && trainingDay != null)
			{
				CompleteTrainingDayWithExercise(trainingDay);
			}

			return trainingDay;
		}

		internal List<TrainingDay> FindTrainingDay(TrainingDayCriteria trainingDayCriteria, TrainingDayScenario trainingDayScenario)
		{
			var trainingDays = _trainingDayModule.Find(trainingDayCriteria);

			if (trainingDayScenario != null && trainingDayScenario.ManageExercise && trainingDays != null)
			{
				foreach (var trainingDay in trainingDays)
				{
					CompleteTrainingDayWithExercise(trainingDay);
				}
			}

			return trainingDays;
		}

		internal TrainingDay UpdateTrainingDay(TrainingDay trainingDay, TrainingDayScenario trainingDayScenario)
		{
			TrainingDay trainingDayResult = null;

			trainingDayResult = _trainingDayModule.Update(trainingDay);

            if (trainingDayScenario != null && trainingDayScenario.ManageExercise)
            {
                var trainingExerciseManager = new TrainingExerciseManager(_dbContext);

                var trainingExerciseCriteria = new TrainingExerciseCriteria()
                {
                    UserId = new StringCriteria() { Equal = trainingDay.UserId },
                    Year = new IntegerCriteria() { Equal = trainingDay.Year },
                    WeekOfYear = new IntegerCriteria() { Equal = trainingDay.WeekOfYear },
                    DayOfWeek = new IntegerCriteria() { Equal = trainingDay.DayOfWeek },
                    TrainingDayId = new IntegerCriteria() { Equal = trainingDay.TrainingDayId }
                };
                var trainingExercisesDb = trainingExerciseManager.FindTrainingExercise(trainingExerciseCriteria);
                if (trainingExercisesDb != null && trainingExercisesDb.Count > 0)
                {
                    foreach (var trainingExerciseDb in trainingExercisesDb)
                        trainingExerciseManager.DeleteTrainingExercise(trainingExerciseDb);
                }

                if (trainingDay.TrainingExercises != null)
                {
                    trainingDayResult.TrainingExercises = new List<TrainingExercise>();
                    foreach (var trainingExercise in trainingDay.TrainingExercises)
                    {
                        trainingDayResult.TrainingExercises.Add(trainingExerciseManager.UpdateTrainingExercise(trainingExercise, true));
                    }
                }
            }

			return trainingDayResult;
		}

		internal void DeleteTrainingDay(TrainingDay trainingDay)
		{
			_trainingDayModule.Delete(trainingDay);

			if (trainingDay.TrainingExercises != null)
			{
				var trainingExerciseManager = new TrainingExerciseManager(_dbContext);
				foreach (var trainingExercise in trainingDay.TrainingExercises)
				{
					trainingExerciseManager.DeleteTrainingExercise(trainingExercise);
				}
			}
		}
	}
}

