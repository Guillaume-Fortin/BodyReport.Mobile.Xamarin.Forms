using System;

namespace BodyReportMobile.Core.Framework
{
	public interface ISecurity
	{
        void RemoveUserInfo();
        void SaveUserInfo(string userId, string userName, string password);
		bool GetUserInfo(out string userId, out string userName, out string password);
    }
}

