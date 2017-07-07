using System;
using BodyReport.Message;
using System.Collections.Generic;
using BodyReportMobile.Core.Crud.Module;
using BodyReportMobile.Core.ServiceLayers;
using BodyReportMobile.Core.Data;
using BodyReport.Framework;

namespace BodyReportMobile.Core.Manager
{
	public class TrainingDayManager : BodyReportManager
    {
		TrainingDayModule _trainingDayModule = null;
        UserInfoService _userInfosService = null;

        public TrainingDayManager(ApplicationDbContext dbContext) : base(dbContext)
		{
			_trainingDayModule = new TrainingDayModule(DbContext);
            _userInfosService = new UserInfoService(DbContext);
        }

		internal TrainingDay CreateTrainingDay(TrainingDay trainingDay)
		{
			TrainingDay trainingDayResult = null;
			trainingDayResult = _trainingDayModule.Create(trainingDay, AppUtils.GetUserUnit(_userInfosService, trainingDay.UserId));

			if (trainingDay.TrainingExercises != null)
			{
				var trainingExerciseService = new TrainingExerciseService(DbContext);
				trainingDayResult.TrainingExercises = new List<TrainingExercise>();
				foreach (var trainingExercise in trainingDay.TrainingExercises)
				{
					trainingDayResult.TrainingExercises.Add(trainingExerciseService.CreateTrainingExercise(trainingExercise));
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
				var trainingExerciseService = new TrainingExerciseService(DbContext);
				trainingJournalDay.TrainingExercises = trainingExerciseService.FindTrainingExercise(trainingExerciseCriteria);
			}
		}

		internal TrainingDay GetTrainingDay(TrainingDayKey key, TrainingDayScenario scenario)
		{
			var trainingDay = _trainingDayModule.Get(key, AppUtils.GetUserUnit(_userInfosService, key.UserId));

			if (scenario != null && scenario.ManageExercise && trainingDay != null)
			{
				CompleteTrainingDayWithExercise(trainingDay);
			}

			return trainingDay;
		}

		internal List<TrainingDay> FindTrainingDay(TUnitType userUnit, TrainingDayCriteria trainingDayCriteria, TrainingDayScenario trainingDayScenario)
		{
			var trainingDays = _trainingDayModule.Find(userUnit, trainingDayCriteria);

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

			trainingDayResult = _trainingDayModule.Update(trainingDay, AppUtils.GetUserUnit(_userInfosService, trainingDay.UserId));
            
            if (trainingDayScenario != null && trainingDayScenario.ManageExercise)
            {
                var trainingExerciseService = new TrainingExerciseService(DbContext);
                
                var trainingExerciseCriteria = new TrainingExerciseCriteria()
                {
                    UserId = new StringCriteria() { Equal = trainingDay.UserId },
                    Year = new IntegerCriteria() { Equal = trainingDay.Year },
                    WeekOfYear = new IntegerCriteria() { Equal = trainingDay.WeekOfYear },
                    DayOfWeek = new IntegerCriteria() { Equal = trainingDay.DayOfWeek },
                    TrainingDayId = new IntegerCriteria() { Equal = trainingDay.TrainingDayId }
                };
                var trainingExercisesDb = trainingExerciseService.FindTrainingExercise(trainingExerciseCriteria);
                if (trainingExercisesDb != null && trainingExercisesDb.Count > 0)
                {
                    foreach (var trainingExerciseDb in trainingExercisesDb)
                        trainingExerciseService.DeleteTrainingExercise(trainingExerciseDb);
                }

                if (trainingDay.TrainingExercises != null)
                {
                    trainingDayResult.TrainingExercises = new List<TrainingExercise>();
                    foreach (var trainingExercise in trainingDay.TrainingExercises)
                    {
                        trainingDayResult.TrainingExercises.Add(trainingExerciseService.UpdateTrainingExercise(trainingExercise, true));
                    }
                }
            }

			return trainingDayResult;
		}

		internal void DeleteTrainingDay(TrainingDayKey key)
		{
            var trainingDayScenario = new TrainingDayScenario() { ManageExercise = true };
            var trainingDay = GetTrainingDay(key, trainingDayScenario);
            if (trainingDay != null)
            {
                _trainingDayModule.Delete(trainingDay);

                if (trainingDay.TrainingExercises != null)
                {
                    var trainingExerciseService = new TrainingExerciseService(DbContext);
                    foreach (var trainingExercise in trainingDay.TrainingExercises)
                    {
                        trainingExerciseService.DeleteTrainingExercise(trainingExercise);
                    }
                }
            }
		}
	}
}

