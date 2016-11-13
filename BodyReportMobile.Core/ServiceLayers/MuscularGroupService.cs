using BodyReport.Message;
using BodyReportMobile.Core.Manager;
using SQLite.Net;
using System.Collections.Generic;

namespace BodyReportMobile.Core.ServiceLayers
{
    public class MuscularGroupService : LocalService
    {
        public MuscularGroupService(SQLiteConnection dbContext) : base(dbContext)
        {
        }

        public List<MuscularGroup> FindMuscularGroups(MuscularGroupCriteria muscularGroupCriteria = null)
        {
            return GetMuscularGroupManager().FindMuscularGroups(muscularGroupCriteria);
        }

        public MuscularGroup GetMuscularGroup(MuscularGroupKey key)
        {
            return GetMuscularGroupManager().GetMuscularGroup(key);
        }

        public MuscularGroup UpdateMuscularGroup(MuscularGroup muscularGroup)
        {
            MuscularGroup result;
            BeginTransaction();
            try
            {
                result = GetMuscularGroupManager().UpdateMuscularGroup(muscularGroup);
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

        public List<MuscularGroup> UpdateMuscularGroupList(List<MuscularGroup> muscularGroupList)
        {
            List<MuscularGroup> list = new List<MuscularGroup>();
            BeginTransaction();
            try
            {
                foreach (var muscularGroup in muscularGroupList)
                {
                    list.Add(GetMuscularGroupManager().UpdateMuscularGroup(muscularGroup));
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
