using System;
using BodyReport.Message;
using BodyReportMobile.Core.Crud.Module;
using SQLite.Net;
using System.Collections.Generic;

namespace BodyReportMobile.Core.ServiceManagers
{
	public class TrainingExerciseSetManager : ServiceManager
	{
		TrainingExerciseSetModule _trainingExerciseSetModule = null;

		public TrainingExerciseSetManager(SQLiteConnection dbContext) : base(dbContext)
		{
			_trainingExerciseSetModule = new TrainingExerciseSetModule(_dbContext);
		}

		public TrainingExerciseSet CreateTrainingExerciseSet(TrainingExerciseSet trainingExerciseSet)
		{
			return _trainingExerciseSetModule.Create(trainingExerciseSet);
		}

		public TrainingExerciseSet UpdateTrainingExerciseSet(TrainingExerciseSet trainingExerciseSet)
		{
			return _trainingExerciseSetModule.Update(trainingExerciseSet);
		}

		public TrainingExerciseSet GetTrainingExerciseSet(TrainingExerciseSetKey key)
		{
			return _trainingExerciseSetModule.Get(key);
		}

		public List<TrainingExerciseSet> FindTrainingExerciseSet(TrainingExerciseSetCriteria trainingExerciseSetCriteria)
		{
			return _trainingExerciseSetModule.Find(trainingExerciseSetCriteria);
		}

		public void DeleteTrainingExerciseSet(TrainingExerciseSet trainingExerciseSet)
		{
			_trainingExerciseSetModule.Delete(trainingExerciseSet);
		}
	}
}

