using BodyReport.Message;
using System.Collections.Generic;
using System;
using BodyReportMobile.Core.Data;

namespace BodyReportMobile.Core.ServiceLayers
{
    public class TrainingWeekService : LocalService
    {
        public TrainingWeekService(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public TrainingWeek CreateTrainingWeek(TrainingWeek trainingWeek)
        {
            TrainingWeek result = null;
            BeginTransaction();
            try
            {
                result = GetTrainingWeekManager().CreateTrainingWeek(trainingWeek);
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

        public TrainingWeek GetTrainingWeek(TrainingWeekKey key, TrainingWeekScenario trainingWeekScenario)
        {
            return GetTrainingWeekManager().GetTrainingWeek(key, trainingWeekScenario);
        }

        public List<TrainingWeek> FindTrainingWeek(TrainingWeekCriteria trainingWeekCriteria, TrainingWeekScenario trainingWeekScenario)
        {
            return GetTrainingWeekManager().FindTrainingWeek(trainingWeekCriteria, trainingWeekScenario);
        }

        public TrainingWeek UpdateTrainingWeek(TrainingWeek trainingWeek, TrainingWeekScenario trainingWeekScenario)
        {
            TrainingWeek result = null;
            BeginTransaction();
            try
            {
                result = GetTrainingWeekManager().UpdateTrainingWeek(trainingWeek, trainingWeekScenario);
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

        public List<TrainingWeek> UpdateTrainingWeekList(List<TrainingWeek> trainingWeekList, TrainingWeekScenario trainingWeekScenario)
        {
            if (trainingWeekList == null)
                return null;

            List<TrainingWeek> result = new List<TrainingWeek>();
            BeginTransaction();
            try
            {
                foreach (var trainingWeek in trainingWeekList)
                {
                    result.Add(GetTrainingWeekManager().UpdateTrainingWeek(trainingWeek, trainingWeekScenario));
                }
                CommitTransaction();
            }
            catch(Exception exception)
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

        public void DeleteTrainingWeek(TrainingWeekKey key)
        {
            BeginTransaction();
            try
            {
                GetTrainingWeekManager().DeleteTrainingWeek(key);
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

        public void DeleteTrainingWeekList(List<TrainingWeekKey> keyList)
        {
            BeginTransaction();
            try
            {
                foreach(var key in keyList)
                    GetTrainingWeekManager().DeleteTrainingWeek(key);
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
