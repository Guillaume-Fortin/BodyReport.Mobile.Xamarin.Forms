using BodyReport.Message;
using BodyReportMobile.Core.ServiceManagers;
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
            return _manager.GetBodyExercise(key);
        }

        public List<BodyExercise> FindBodyExercises(BodyExerciseCriteria bodyExerciseCriteria = null)
        {
            return _manager.FindBodyExercises(bodyExerciseCriteria);
        }

        public BodyExercise CreateBodyExercise(BodyExercise bodyExercise)
        {
            return _manager.CreateBodyExercise(bodyExercise);
        }

        internal void DeleteBodyExercise(BodyExerciseKey key)
        {
            _manager.DeleteBodyExercise(key);
        }

        internal BodyExercise UpdateBodyExercise(BodyExercise bodyExercise)
        {
            return _manager.UpdateBodyExercise(bodyExercise);
        }

        internal List<BodyExercise> UpdateBodyExerciseList(List<BodyExercise> bodyExerciseList)
        {
            List<BodyExercise> list = new List<BodyExercise>();
            _dbContext.BeginTransaction();
            try
            {
                foreach (var bodyExercise in bodyExerciseList)
                {
                    list.Add(_manager.UpdateBodyExercise(bodyExercise));
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
