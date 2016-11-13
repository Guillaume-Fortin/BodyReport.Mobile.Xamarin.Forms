using BodyReport.Message;
using BodyReportMobile.Core.Manager;
using SQLite.Net;
using System.Collections.Generic;

namespace BodyReportMobile.Core.ServiceLayers
{
    public class BodyExerciseService : LocalService
    {
        public BodyExerciseService(SQLiteConnection dbContext) : base(dbContext)
        {
        }

        public BodyExercise GetBodyExercise(BodyExerciseKey key)
        {
            return GetBodyExerciseManager().Get(key);
        }

        public List<BodyExercise> FindBodyExercises(BodyExerciseCriteria bodyExerciseCriteria = null)
        {
            return GetBodyExerciseManager().Find(bodyExerciseCriteria);
        }

        public BodyExercise CreateBodyExercise(BodyExercise bodyExercise)
        {
            return GetBodyExerciseManager().Create(bodyExercise);
        }

        public BodyExercise UpdateBodyExercise(BodyExercise bodyExercise)
        {
            BodyExercise result = null;
            BeginTransaction();
            try
            {
                result = GetBodyExerciseManager().Update(bodyExercise);
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

        public List<BodyExercise> UpdateBodyExerciseList(List<BodyExercise> bodyExerciseList)
        {
            List<BodyExercise> list = new List<BodyExercise>();
            BeginTransaction();
            try
            {
                foreach (var bodyExercise in bodyExerciseList)
                {
                    list.Add(GetBodyExerciseManager().Update(bodyExercise));
                }
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

            return list;
        }
        
        public void DeleteBodyExercise(BodyExerciseKey key)
        {
            BeginTransaction();
            try
            {
                GetBodyExerciseManager().Delete(key);
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
