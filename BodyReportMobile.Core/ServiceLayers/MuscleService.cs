using BodyReport.Message;
using BodyReportMobile.Core.Manager;
using SQLite.Net;
using System.Collections.Generic;

namespace BodyReportMobile.Core.ServiceLayers
{
    public class MuscleService : LocalService
    {
        public MuscleService(SQLiteConnection dbContext) : base(dbContext)
        {
        }

        public List<Muscle> FindMuscles(MuscleCriteria muscleCriteria = null)
        {
            return GetMuscleManager().Find(muscleCriteria);
        }

        public Muscle GetMuscle(MuscleKey key)
        {
            return GetMuscleManager().Get(key);
        }

        public Muscle UpdateMuscle(Muscle muscle)
        {
            Muscle result;
            BeginTransaction();
            try
            {
                result = GetMuscleManager().Update(muscle);
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

        public List<Muscle> UpdateMuscleList(List<Muscle> muscleList)
        {
            List<Muscle> list = new List<Muscle>();
            BeginTransaction();
            try
            {
                foreach (var muscle in muscleList)
                {
                    list.Add(GetMuscleManager().Update(muscle));
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
    }
}
