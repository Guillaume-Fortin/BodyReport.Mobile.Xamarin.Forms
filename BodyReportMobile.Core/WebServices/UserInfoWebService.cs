using System;
using System.Threading.Tasks;
using Message;
using BodyReportMobile.Core.Framework;

namespace BodyReportMobile.Core.WebServices
{
	public class UserInfoWebService
	{
		public static async Task<UserInfo> GetUserInfo (string userId)
		{
			return await HttpConnector.Instance.GetAsync<UserInfo> ("api/Muscles/Find");
		}
	}
}

