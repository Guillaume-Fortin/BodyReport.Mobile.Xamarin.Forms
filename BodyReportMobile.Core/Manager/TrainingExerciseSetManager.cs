using System;
using BodyReport.Message;
using BodyReportMobile.Core.Crud.Module;
using System.Collections.Generic;
using BodyReportMobile.Core.Data;

namespace BodyReportMobile.Core.Manager
{
	public class TrainingExerciseSetManager : BodyReportManager
    {
		TrainingExerciseSetModule _trainingExerciseSetModule = null;

		public TrainingExerciseSetManager(ApplicationDbContext dbContext) : base(dbContext)
		{
			_trainingExerciseSetModule = new TrainingExerciseSetModule(DbContext);
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

        public List<TrainingExerciseSet> FindTrainingExerciseSet(CriteriaList<TrainingExerciseSetCriteria> trainingExerciseSetCriteriaList)
        {
            return _trainingExerciseSetModule.Find(trainingExerciseSetCriteriaList);
        }

        public void DeleteTrainingExerciseSet(TrainingExerciseSetKey key)
		{
			_trainingExerciseSetModule.Delete(key);
		}
	}
}

