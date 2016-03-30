using System;

namespace BodyReportMobile.Core
{
	public interface ISecurity
	{
		void SaveUserInfo(string userName, string password);
		bool GetUserInfo(out string userName, out string password);
	}
}

