﻿using BodyReportMobile.Core.Crud.Module;
using BodyReport.Message;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Manager
{
    public class UserInfoManager : BodyReportManager
    {
        UserInfoModule _userInfoModule = null;

        public UserInfoManager(SQLiteConnection dbContext) : base(dbContext)
        {
            _userInfoModule = new UserInfoModule(DbContext);
        }

        internal UserInfo GetUserInfo(UserInfoKey key)
        {
            return _userInfoModule.Get(key);
        }

        public List<UserInfo> FindUserInfos(UserInfoCriteria userInfoCriteria = null)
        {
            return _userInfoModule.Find(userInfoCriteria);
        }
        
        internal UserInfo UpdateUserInfo(UserInfo userInfo)
        {
            return _userInfoModule.Update(userInfo);
        }

        internal void DeleteUserInfo(UserInfoKey key)
        {
            _userInfoModule.Delete(key);
        }
    }
}
