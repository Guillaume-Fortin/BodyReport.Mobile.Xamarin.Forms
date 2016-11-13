using BodyReport.Message;
using BodyReportMobile.Core.Manager;
using SQLite.Net;
using System.Collections.Generic;

namespace BodyReportMobile.Core.ServiceLayers
{
    public class UserInfoService : LocalService
    {
        public UserInfoService(SQLiteConnection dbContext) : base(dbContext)
        {
        }

        public UserInfo GetUserInfo(UserInfoKey key)
        {
            return GetUserInfoManager().GetUserInfo(key);
        }

        public List<UserInfo> FindUserInfos(UserInfoCriteria userInfoCriteria = null)
        {
            return GetUserInfoManager().FindUserInfos(userInfoCriteria);
        }

        public UserInfo UpdateUserInfo(UserInfo userInfo)
        {
            UserInfo result = null;
            BeginTransaction();
            try
            {
                result = GetUserInfoManager().UpdateUserInfo(userInfo);
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

        public void DeleteUserInfo(UserInfoKey key)
        {
            BeginTransaction();
            try
            {
                GetUserInfoManager().DeleteUserInfo(key);
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

        public void DeleteUserInfoList(List<UserInfoKey> keyList)
        {
            if (keyList == null)
                return;
            BeginTransaction();
            try
            {
                foreach (var key in keyList)
                {
                    GetUserInfoManager().DeleteUserInfo(key);
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
        }
    }
}
