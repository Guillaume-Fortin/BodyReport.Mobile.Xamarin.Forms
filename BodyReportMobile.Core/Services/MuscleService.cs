using BodyReport.Message;
using BodyReportMobile.Core.ServiceManagers;
using SQLite.Net;
using System.Collections.Generic;

namespace BodyReportMobile.Core.Services
{
    public class MuscleService : LocalService
    {
        MuscleManager _manager;

        public MuscleService(SQLiteConnection dbContext) : base(dbContext)
        {
            _manager = new MuscleManager(_dbContext);
        }

        public List<Muscle> FindMuscles(MuscleCriteria muscleCriteria = null)
        {
            return _manager.Find(muscleCriteria);
        }

        internal Muscle GetMuscle(MuscleKey key)
        {
            return _manager.Get(key);
        }

        internal Muscle UpdateMuscle(Muscle muscle)
        {
            return _manager.Update(muscle);
        }

        internal List<Muscle> UpdateMuscleList(List<Muscle> muscleList)
        {
            List<Muscle> list = new List<Muscle>();
            _dbContext.BeginTransaction();
            try
            {
                foreach (var muscle in muscleList)
                {
                    list.Add(_manager.Update(muscle));
                }
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
    }
}
