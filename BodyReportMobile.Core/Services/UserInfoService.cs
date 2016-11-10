using BodyReport.Message;
using BodyReportMobile.Core.Manager;
using SQLite.Net;
using System.Collections.Generic;

namespace BodyReportMobile.Core.Services
{
    public class UserInfoService : LocalService
    {
        UserInfoManager _manager;

        public UserInfoService(SQLiteConnection dbContext) : base(dbContext)
        {
            _manager = new UserInfoManager(_dbContext);
        }

        internal UserInfo GetUserInfo(UserInfoKey key)
        {
            return _manager.GetUserInfo(key);
        }

        public List<UserInfo> FindUserInfos(UserInfoCriteria userInfoCriteria = null)
        {
            return _manager.FindUserInfos(userInfoCriteria);
        }

        public void DeleteUserInfo(UserInfo userInfo)
        {
            _manager.DeleteUserInfo(userInfo);
        }

        public void DeleteUserInfoList(List<UserInfo> userInfoList)
        {
            _dbContext.BeginTransaction();
            try
            {
                foreach (var userInfo in userInfoList)
                {
                    _manager.DeleteUserInfo(userInfo);
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
        }

        public UserInfo UpdateUserInfo(UserInfo userInfo)
        {
            return _manager.UpdateUserInfo(userInfo);
        }
    }
}
