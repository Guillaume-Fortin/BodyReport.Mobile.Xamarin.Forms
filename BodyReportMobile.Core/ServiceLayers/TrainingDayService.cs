using BodyReport.Message;
using BodyReportMobile.Core.Data;
using System.Collections.Generic;

namespace BodyReportMobile.Core.ServiceLayers
{
    public class TrainingDayService : LocalService
    {
        public TrainingDayService(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public TrainingDay CreateTrainingDay(TrainingDay trainingDay)
        {
            TrainingDay result = null;
            BeginTransaction();
            try
            {
                result = GetTrainingDayManager().CreateTrainingDay(trainingDay);
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

        public TrainingDay GetTrainingDay(TrainingDayKey key, TrainingDayScenario scenario)
        {
            TrainingDay result = null;
            BeginTransaction();
            try
            {
                result = GetTrainingDayManager().GetTrainingDay(key, scenario);
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

        public List<TrainingDay> FindTrainingDay(TUnitType userUnit, TrainingDayCriteria trainingDayCriteria, TrainingDayScenario trainingDayScenario)
        {
            List<TrainingDay> result;
            BeginTransaction();
            try
            {
                result = GetTrainingDayManager().FindTrainingDay(userUnit, trainingDayCriteria, trainingDayScenario);
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

        public TrainingDay UpdateTrainingDay(TrainingDay trainingDay, TrainingDayScenario trainingDayScenario)
        {
            TrainingDay result;
            BeginTransaction();
            try
            {
                result = GetTrainingDayManager().UpdateTrainingDay(trainingDay, trainingDayScenario);
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

        public void DeleteTrainingDay(TrainingDay trainingDay)
        {
            BeginTransaction();
            try
            {
                GetTrainingDayManager().DeleteTrainingDay(trainingDay);
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

        public void ChangeUnitForTrainingExercises(TrainingDay trainingDay, TUnitType oldUnit)
        {
            GetTrainingDayManager().ChangeUnitForTrainingExercises(trainingDay, oldUnit);
        }
    }
}
