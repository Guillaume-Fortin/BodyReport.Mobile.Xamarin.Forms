using BodyReport.Message;
using System.Collections.Generic;
using SQLite.Net;
using BodyReportMobile.Core.Manager;

namespace BodyReportMobile.Core.Services
{
    public class TrainingWeekService : LocalService
    {
        TrainingWeekManager _manager;

        public TrainingWeekService(SQLiteConnection dbContext) : base(dbContext)
        {
            _manager = new TrainingWeekManager(_dbContext);
        }

        public TrainingWeek CreateTrainingWeek(TrainingWeek trainingWeek)
        {
            _dbContext.BeginTransaction();
            try
            {
                return _manager.CreateTrainingWeek(trainingWeek);
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

        public TrainingWeek UpdateTrainingWeek(TrainingWeek trainingWeek, TrainingWeekScenario trainingWeekScenario)
        {
            _dbContext.BeginTransaction();
            try
            {
                return _manager.UpdateTrainingWeek(trainingWeek, trainingWeekScenario);
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

        public List<TrainingWeek> UpdateTrainingWeekList(List<TrainingWeek> trainingWeekList, TrainingWeekScenario trainingWeekScenario)
        {
            if (trainingWeekList == null)
                return null;
            List<TrainingWeek> list = new List<TrainingWeek>();
            _dbContext.BeginTransaction();
            try
            {
                foreach (var trainingWeek in trainingWeekList)
                    list.Add(_manager.UpdateTrainingWeek(trainingWeek, trainingWeekScenario));
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
            return list;
        }

        public void DeleteTrainingWeek(TrainingWeekKey key)
        {
            _dbContext.BeginTransaction();
            try
            {
                _manager.DeleteTrainingWeek(key);
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

        public void DeleteTrainingWeekList(List<TrainingWeek> trainingWeekList)
        {
            _dbContext.BeginTransaction();
            try
            {
                foreach(var trainingWeek in trainingWeekList)
                    _manager.DeleteTrainingWeek(trainingWeek);
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

        public TrainingWeek GetTrainingWeek(TrainingWeekKey key, TrainingWeekScenario trainingWeekScenario)
        {
            return _manager.GetTrainingWeek(key, trainingWeekScenario);
        }

        public List<TrainingWeek> FindTrainingWeek(TrainingWeekCriteria trainingWeekCriteria, TrainingWeekScenario trainingWeekScenario)
        {
            return _manager.FindTrainingWeek(trainingWeekCriteria, trainingWeekScenario);
        }
    }
}
