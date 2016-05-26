using BodyReportMobile.Core.Crud.Module;
using Message;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.ServiceManagers
{
    public class UserInfoManager : ServiceManager
    {
        UserInfoModule _userInfoModule = null;

        public UserInfoManager(SQLiteConnection dbContext) : base(dbContext)
        {
            _userInfoModule = new UserInfoModule(_dbContext);
        }

        internal UserInfo GetUserInfo(UserInfoKey key)
        {
            return _userInfoModule.Get(key);
        }

        public List<UserInfo> FindUserInfos(UserInfoCriteria userInfoCriteria = null)
        {
            return _userInfoModule.Find(userInfoCriteria);
        }

        internal void DeleteUserInfo(UserInfoKey key)
        {
            _userInfoModule.Delete(key);
        }

        internal UserInfo UpdateUserInfo(UserInfo userInfo)
        {
            return _userInfoModule.Update(userInfo);
        }
    }
}
