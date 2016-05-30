using System;
using System.Threading.Tasks;
using BodyReport.Message;
using BodyReportMobile.Core.Framework;
using System.Collections.Generic;

namespace BodyReportMobile.Core.WebServices
{
	public class UserInfoWebService
	{
        public static async Task<User> GetUserAsync(string userId)
        {
            Dictionary<string, string> datas = new Dictionary<string, string>();
            datas.Add("userId", userId);
            return await HttpConnector.Instance.GetAsync<User>("Api/Account/GetUser", datas);
        }

        public static async Task<UserInfo> GetUserInfoAsync (UserInfoKey userInfoKey)
		{
            Dictionary<string, string> datas = new Dictionary<string, string>();
            datas.Add("userId", userInfoKey.UserId);
            return await HttpConnector.Instance.GetAsync<UserInfo> ("Api/Account/GetUserInfo", datas);
        }

        public static async Task<UserInfo> UpdateUserInfoAsync(UserInfo userInfo)
        {
            return await HttpConnector.Instance.PostAsync<UserInfo, UserInfo>("Api/Account/UpdateUserInfo", userInfo);
        }
    }
}

