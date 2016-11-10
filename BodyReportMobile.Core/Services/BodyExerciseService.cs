using BodyReport.Message;
using BodyReportMobile.Core.Manager;
using SQLite.Net;
using System.Collections.Generic;

namespace BodyReportMobile.Core.Services
{
    public class BodyExerciseService : LocalService
    {
        BodyExerciseManager _manager;

        public BodyExerciseService(SQLiteConnection dbContext) : base(dbContext)
        {
            _manager = new BodyExerciseManager(_dbContext);
        }

        public BodyExercise GetBodyExercise(BodyExerciseKey key)
        {
            return _manager.Get(key);
        }

        public List<BodyExercise> FindBodyExercises(BodyExerciseCriteria bodyExerciseCriteria = null)
        {
            return _manager.Find(bodyExerciseCriteria);
        }

        public BodyExercise CreateBodyExercise(BodyExercise bodyExercise)
        {
            return _manager.Create(bodyExercise);
        }

        internal void DeleteBodyExercise(BodyExerciseKey key)
        {
            _manager.Delete(key);
        }

        internal BodyExercise UpdateBodyExercise(BodyExercise bodyExercise)
        {
            return _manager.Update(bodyExercise);
        }

        internal List<BodyExercise> UpdateBodyExerciseList(List<BodyExercise> bodyExerciseList)
        {
            List<BodyExercise> list = new List<BodyExercise>();
            _dbContext.BeginTransaction();
            try
            {
                foreach (var bodyExercise in bodyExerciseList)
                {
                    list.Add(_manager.Update(bodyExercise));
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
