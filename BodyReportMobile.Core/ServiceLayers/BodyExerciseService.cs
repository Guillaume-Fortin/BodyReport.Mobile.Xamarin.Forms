using BodyReport.Message;
using BodyReportMobile.Core.Manager;
using SQLite.Net;
using System;
using System.Collections.Generic;

namespace BodyReportMobile.Core.ServiceLayers
{
    public class BodyExerciseService : LocalService
    {
        private const string _cacheName = "BodyExercisesCache";

        public BodyExerciseService(SQLiteConnection dbContext) : base(dbContext)
        {
        }
        
        public BodyExercise CreateBodyExercise(BodyExercise bodyExercise)
        {
            //invalidate cache
            InvalidateCache(_cacheName);
            BodyExercise result = null;
            BeginTransaction();
            try
            {
                result = GetBodyExerciseManager().CreateBodyExercise(bodyExercise);
                CommitTransaction();
                //invalidate cache
                InvalidateCache(_cacheName);
            }
            catch (Exception exception)
            {
                RollbackTransaction();
                throw exception;
            }
            finally
            {
                EndTransaction();
            }
            return result;
        }

        public BodyExercise GetBodyExercise(BodyExerciseKey key)
        {
            BodyExercise bodyExercise = null;
            string cacheKey = key == null ? "BodyExerciseKey_null" : key.GetCacheKey();
            if (key != null && !TryGetCacheData(cacheKey, out bodyExercise, _cacheName))
            {
                bodyExercise = GetBodyExerciseManager().GetBodyExercise(key);
                SetCacheData(_cacheName, cacheKey, bodyExercise);
            }
            return bodyExercise;
        }

        public List<BodyExercise> FindBodyExercises(BodyExerciseCriteria criteria = null)
        {
            List<BodyExercise> bodyExerciseList = null;
            string cacheKey = criteria == null ? "BodyExerciseCriteria_null" : criteria.GetCacheKey();
            if (!TryGetCacheData(cacheKey, out bodyExerciseList, _cacheName))
            {
                bodyExerciseList = GetBodyExerciseManager().FindBodyExercises(criteria);
                SetCacheData(_cacheName, cacheKey, bodyExerciseList);
            }
            return bodyExerciseList;
        }

        public BodyExercise UpdateBodyExercise(BodyExercise bodyExercise)
        {
            BodyExercise result = null;
            BeginTransaction();
            try
            {
                result = GetBodyExerciseManager().Update(bodyExercise);
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

        public List<BodyExercise> UpdateBodyExerciseList(List<BodyExercise> bodyExercises)
        {
            List<BodyExercise> results = null;
            BeginTransaction();
            try
            {
                if (bodyExercises != null && bodyExercises.Count > 0)
                {
                    results = new List<BodyExercise>();
                    foreach (var bodyExercise in bodyExercises)
                    {
                        results.Add(GetBodyExerciseManager().Update(bodyExercise));
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

            return results;
        }
        
        public void DeleteBodyExercise(BodyExerciseKey key)
        {
            BeginTransaction();
            try
            {
                GetBodyExerciseManager().Delete(key);
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
        }
    }
}
