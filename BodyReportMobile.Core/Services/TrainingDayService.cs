using BodyReport.Message;
using BodyReportMobile.Core.ServiceManagers;
using SQLite.Net;
using System.Collections.Generic;

namespace BodyReportMobile.Core.Services
{
    public class TrainingDayService : LocalService
    {
        TrainingDayManager _manager;

        public TrainingDayService(SQLiteConnection dbContext) : base(dbContext)
        {
            _manager = new TrainingDayManager(_dbContext);
        }

        internal TrainingDay CreateTrainingDay(TrainingDay trainingDay)
        {
            _dbContext.BeginTransaction();
            try
            {
                return _manager.CreateTrainingDay(trainingDay);
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
        
        internal TrainingDay GetTrainingDay(TrainingDayKey key, bool manageExercise)
        {
            _dbContext.BeginTransaction();
            try
            {
                return _manager.GetTrainingDay(key, manageExercise);
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

        public List<TrainingDay> FindTrainingDay(TrainingDayCriteria trainingDayCriteria, TrainingDayScenario trainingDayScenario)
        {
            _dbContext.BeginTransaction();
            try
            {
                return _manager.FindTrainingDay(trainingDayCriteria, trainingDayScenario);
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

        public TrainingDay UpdateTrainingDay(TrainingDay trainingDay, TrainingDayScenario trainingDayScenario)
        {
            _dbContext.BeginTransaction();
            try
            {
                return _manager.UpdateTrainingDay(trainingDay, trainingDayScenario);
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

        public void DeleteTrainingDay(TrainingDay trainingDay)
        {
            _dbContext.BeginTransaction();
            try
            {
                _manager.DeleteTrainingDay(trainingDay);
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
