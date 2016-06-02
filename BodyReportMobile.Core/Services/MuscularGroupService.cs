using BodyReport.Message;
using BodyReportMobile.Core.ServiceManagers;
using SQLite.Net;
using System.Collections.Generic;

namespace BodyReportMobile.Core.Services
{
    public class MuscularGroupService : LocalService
    {
        MuscularGroupManager _manager;

        public MuscularGroupService(SQLiteConnection dbContext) : base(dbContext)
        {
            _manager = new MuscularGroupManager(_dbContext);
        }

        public List<MuscularGroup> FindMuscularGroups(MuscularGroupCriteria muscularGroupCriteria = null)
        {
            return _manager.FindMuscularGroups(muscularGroupCriteria);
        }

        public MuscularGroup GetMuscularGroup(MuscularGroupKey key)
        {
            return _manager.GetMuscularGroup(key);
        }

        public void DeleteMuscularGroup(MuscularGroupKey key)
        {
            _manager.DeleteMuscularGroup(key);
        }

        public MuscularGroup UpdateMuscularGroup(MuscularGroup muscularGroup)
        {
            return _manager.UpdateMuscularGroup(muscularGroup);
        }

        internal List<MuscularGroup> UpdateMuscularGroupList(List<MuscularGroup> muscularGroupList)
        {
            List<MuscularGroup> list = new List<MuscularGroup>();
            _dbContext.BeginTransaction();
            try
            {
                foreach (var muscularGroup in muscularGroupList)
                {
                    list.Add(_manager.UpdateMuscularGroup(muscularGroup));
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
