using System;
using System.Threading.Tasks;
using Message;

namespace BodyReportMobile.Core
{
	public class UserInfoWebService
	{
		public static async Task<UserInfo> GetUserInfo (string userId)
		{
			return await HttpConnector.Instance.GetAsync<UserInfo> ("api/Muscles/Find");
		}
	}
}

