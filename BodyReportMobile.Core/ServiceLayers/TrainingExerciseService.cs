using BodyReport.Message;
using BodyReportMobile.Core.Data;
using System.Collections.Generic;

namespace BodyReportMobile.Core.ServiceLayers
{
    public class TrainingExerciseService : LocalService
    {
        public TrainingExerciseService(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public TrainingExercise CreateTrainingExercise(TrainingExercise trainingExercise)
        {
            TrainingExercise result;
            BeginTransaction();
            try
            {
                result = GetTrainingExerciseManager().CreateTrainingExercise(trainingExercise);
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                EndTransaction();
            }
            return result;
        }

        public TrainingExercise GetTrainingExercise(TrainingExerciseKey key)
        {
            TrainingExercise result = null;
            BeginTransaction();
            try
            {
                result = GetTrainingExerciseManager().GetTrainingExercise(key);
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                EndTransaction();
            }
            return result;
        }

        public List<TrainingExercise> FindTrainingExercise(TrainingExerciseCriteria trainingExerciseCriteria)
        {
            List<TrainingExercise> result = null;
            BeginTransaction();
            try
            {
                result = GetTrainingExerciseManager().FindTrainingExercise(trainingExerciseCriteria);
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                EndTransaction();
            }
            return result;
        }
        
        public TrainingExercise UpdateTrainingExercise(TrainingExercise trainingExercise, bool manageDeleteLinkItem)
        {
            TrainingExercise result = null;
            BeginTransaction();
            try
            {
                result = GetTrainingExerciseManager().UpdateTrainingExercise(trainingExercise, manageDeleteLinkItem);
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                EndTransaction();
            }
            return result;
        }

        public void DeleteTrainingExercise(TrainingExerciseKey key)
        {
            BeginTransaction();
            try
            {
                GetTrainingExerciseManager().DeleteTrainingExercise(key);
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                EndTransaction();
            }
        }
    }
}
