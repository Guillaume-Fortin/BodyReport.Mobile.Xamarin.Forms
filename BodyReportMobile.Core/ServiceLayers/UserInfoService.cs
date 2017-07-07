using BodyReport.Message;
using BodyReportMobile.Core.Data;
using System.Collections.Generic;

namespace BodyReportMobile.Core.ServiceLayers
{
    public class UserInfoService : LocalService
    {
        private const string _cacheName = "UserInfosCache";
        public UserInfoService(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public UserInfo GetUserInfo(UserInfoKey key)
        {
            UserInfo userInfo = null;
            string cacheKey = key == null ? "UserInfoKey_null" : key.GetCacheKey();
            if (key != null && !TryGetCacheData(cacheKey, out userInfo, _cacheName))
            {
                userInfo = GetUserInfoManager().GetUserInfo(key);
                SetCacheData(_cacheName, cacheKey, userInfo);
            }
            return userInfo;
        }

        public List<UserInfo> FindUserInfos(UserInfoCriteria criteria = null)
        {
            List<UserInfo> userInfoList = null;
            string cacheKey = criteria == null ? "UserInfoCriteria_null" : criteria.GetCacheKey();
            if (!TryGetCacheData(cacheKey, out userInfoList, _cacheName))
            {
                userInfoList = GetUserInfoManager().FindUserInfos(criteria);
                SetCacheData(_cacheName, cacheKey, userInfoList);
            }
            return userInfoList;
        }

        public UserInfo UpdateUserInfo(UserInfo userInfo)
        {
            UserInfo result = null;
            BeginTransaction();
            try
            {
                result = GetUserInfoManager().UpdateUserInfo(userInfo);
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

        public void DeleteUserInfo(UserInfoKey key)
        {
            BeginTransaction();
            try
            {
                GetUserInfoManager().DeleteUserInfo(key);
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
