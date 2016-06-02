using BodyReport.Message;
using BodyReportMobile.Core.ServiceManagers;
using SQLite.Net;
using System.Collections.Generic;

namespace BodyReportMobile.Core.Services
{
    public class TrainingExerciseService : LocalService
    {
        TrainingExerciseManager _manager;

        public TrainingExerciseService(SQLiteConnection dbContext) : base(dbContext)
        {
            _manager = new TrainingExerciseManager(_dbContext);
        }

        public TrainingExercise CreateTrainingExercise(TrainingExercise trainingExercise)
        {
            _dbContext.BeginTransaction();
            try
            {
                return _manager.CreateTrainingExercise(trainingExercise);
            }
            catch
            {
                _dbContext.Rollback();
                throw;
            }
            finally
            {
                _dbContext.Commit();
            }
        }

        public TrainingExercise UpdateTrainingExercise(TrainingExercise trainingExercise, bool manageDeleteLinkItem)
        {
            _dbContext.BeginTransaction();
            try
            {
                return _manager.UpdateTrainingExercise(trainingExercise, manageDeleteLinkItem);
            }
            catch
            {
                _dbContext.Rollback();
                throw;
            }
            finally
            {
                _dbContext.Commit();
            }
        }

        public TrainingExercise GetTrainingExercise(TrainingExerciseKey key)
        {
            _dbContext.BeginTransaction();
            try
            {
                return _manager.GetTrainingExercise(key);
            }
            catch
            {
                _dbContext.Rollback();
                throw;
            }
            finally
            {
                _dbContext.Commit();
            }
        }

        public List<TrainingExercise> FindTrainingExercise(TrainingExerciseCriteria trainingExerciseCriteria)
        {
            _dbContext.BeginTransaction();
            try
            {
                return _manager.FindTrainingExercise(trainingExerciseCriteria);
            }
            catch
            {
                _dbContext.Rollback();
                throw;
            }
            finally
            {
                _dbContext.Commit();
            }
        }

        public void DeleteTrainingExercise(TrainingExercise trainingExercise)
        {
            _dbContext.BeginTransaction();
            try
            {
                _manager.DeleteTrainingExercise(trainingExercise);
            }
            catch
            {
                _dbContext.Rollback();
                throw;
            }
            finally
            {
                _dbContext.Commit();
            }
        }
    }
}
