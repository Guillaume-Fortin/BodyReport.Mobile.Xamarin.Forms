using BodyReport.Message;
using BodyReportMobile.Core.Manager;
using SQLite.Net;
using System.Collections.Generic;

namespace BodyReportMobile.Core.ServiceLayers
{
    public class MuscularGroupService : LocalService
    {
        private const string _cacheName = "MuscularGroupsCache";
        public MuscularGroupService(SQLiteConnection dbContext) : base(dbContext)
        {
        }
        
        public MuscularGroup GetMuscularGroup(MuscularGroupKey key)
        {
            MuscularGroup muscularGroup = null;
            string cacheKey = key == null ? "MuscularGroupKey_null" : key.GetCacheKey();
            if (key != null && !TryGetCacheData(cacheKey, out muscularGroup, _cacheName))
            {
                muscularGroup = GetMuscularGroupManager().GetMuscularGroup(key);
                SetCacheData(_cacheName, cacheKey, muscularGroup);
            }
            return muscularGroup;
        }

        public List<MuscularGroup> FindMuscularGroups(MuscularGroupCriteria criteria = null)
        {
            List<MuscularGroup> muscularGroupList = null;
            string cacheKey = criteria == null ? "MuscularGroupCriteria_null" : criteria.GetCacheKey();
            if (!TryGetCacheData(cacheKey, out muscularGroupList, _cacheName))
            {
                muscularGroupList = GetMuscularGroupManager().FindMuscularGroups(criteria);
                SetCacheData(_cacheName, cacheKey, muscularGroupList);
            }
            return muscularGroupList;
        }

        public MuscularGroup UpdateMuscularGroup(MuscularGroup muscularGroup)
        {
            MuscularGroup result;
            BeginTransaction();
            try
            {
                result = GetMuscularGroupManager().UpdateMuscularGroup(muscularGroup);
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

        public List<MuscularGroup> UpdateMuscularGroupList(List<MuscularGroup> muscularGroups)
        {
            List<MuscularGroup> list = new List<MuscularGroup>();
            BeginTransaction();
            try
            {
                if (muscularGroups != null && muscularGroups.Count > 0)
                {
                    foreach (var muscularGroup in muscularGroups)
                    {
                        list.Add(GetMuscularGroupManager().UpdateMuscularGroup(muscularGroup));
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

        public void DeleteMuscularGroup(MuscularGroupKey key)
        {
            BeginTransaction();
            try
            {
                GetMuscularGroupManager().DeleteMuscularGroup(key);
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
