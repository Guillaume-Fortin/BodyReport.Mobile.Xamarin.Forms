using System;
using System.Threading.Tasks;
using Message;
using BodyReportMobile.Core.Framework;
using System.Collections.Generic;

namespace BodyReportMobile.Core.WebServices
{
	public class UserInfoWebService
	{
		public static async Task<UserInfo> GetUserInfo (UserInfoKey userInfoKey)
		{
            Dictionary<string, string> datas = new Dictionary<string, string>();
            datas.Add("userId", userInfoKey.UserId);
            return await HttpConnector.Instance.GetAsync<UserInfo> ("Api/Account/GetUserInfo", datas);
		}
	}
}

