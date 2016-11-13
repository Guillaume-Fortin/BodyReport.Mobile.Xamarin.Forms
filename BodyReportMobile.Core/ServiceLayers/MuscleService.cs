using BodyReport.Message;
using BodyReportMobile.Core.Manager;
using SQLite.Net;
using System.Collections.Generic;

namespace BodyReportMobile.Core.ServiceLayers
{
    public class MuscleService : LocalService
    {
        private const string _cacheName = "MusclesCache";

        public MuscleService(SQLiteConnection dbContext) : base(dbContext)
        {
        }

        public Muscle GetMuscle(MuscleKey key)
        {
            Muscle muscle = null;
            string cacheKey = key == null ? "MuscleKey_null" : key.GetCacheKey();
            if (key != null && !TryGetCacheData(cacheKey, out muscle, _cacheName))
            {
                muscle = GetMuscleManager().GetMuscle(key);
                SetCacheData(_cacheName, cacheKey, muscle);
            }
            return muscle;
        }

        public List<Muscle> FindMuscles(MuscleCriteria criteria = null)
        {
            List<Muscle> muscleList = null;
            string cacheKey = criteria == null ? "MuscleCriteria_null" : criteria.GetCacheKey();
            if (!TryGetCacheData(cacheKey, out muscleList, _cacheName))
            {
                muscleList = GetMuscleManager().FindMuscles(criteria);
                SetCacheData(_cacheName, cacheKey, muscleList);
            }
            return muscleList;
        }

        public Muscle UpdateMuscle(Muscle muscle)
        {
            Muscle result;
            BeginTransaction();
            try
            {
                result = GetMuscleManager().Update(muscle);
                CommitTransaction();
                //invalidate cache
                InvalidateCache(_cacheName);
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

        public List<Muscle> UpdateMuscleList(List<Muscle> muscles)
        {
            List<Muscle> list = new List<Muscle>();
            BeginTransaction();
            try
            {
                if (muscles != null && muscles.Count > 0)
                {
                    foreach (var muscle in muscles)
                    {
                        list.Add(GetMuscleManager().Update(muscle));
                    }
                    //invalidate cache
                    InvalidateCache(_cacheName);
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
