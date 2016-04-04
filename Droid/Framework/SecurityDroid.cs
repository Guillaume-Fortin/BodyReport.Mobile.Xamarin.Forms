using System;
using BodyReportMobile.Core;

namespace BodyReport.Droid
{
	public class SecurityDroid : ISecurity
	{
		public void SaveUserInfo (string userName, string password)
		{
			// TODO
		}

		public bool GetUserInfo (out string userName, out string password)
		{
			bool result = false;
			password = userName = string.Empty;

			try
			{
                // TODO
            }
            catch
			{
			}

			return result;
		}
	}
}

