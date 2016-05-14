using System;
using Security;
using Foundation;
using BodyReportMobile.Core.Framework;

namespace BodyReport.iOS.Framework
{
	public class SecurityIOS : ISecurity
	{
		public void SaveUserInfo (string userId, string userName, string password)
		{
			var rec = new SecRecord (SecKind.GenericPassword){
				Generic = NSData.FromString ("password")
			};

			SecStatusCode res;
			var match = SecKeyChain.QueryAsRecord (rec, out res);
			if (res == SecStatusCode.Success)
				res = SecKeyChain.Remove (rec);
				
			var s = new SecRecord (SecKind.GenericPassword) {
				Service = "BodyReportUserInfo",
				Account = userId + (char)3 + userName,
				ValueData = NSData.FromString (password),
				Generic = NSData.FromString ("password")
			};

			res = SecKeyChain.Add (s);
		}

		public bool GetUserInfo (out string userId, out string userName, out string password)
		{
			bool result = false;
            userId = password = userName = string.Empty;

			try
			{
				var rec = new SecRecord (SecKind.GenericPassword){
					Service = "BodyReportUserInfo",
					Generic = NSData.FromString ("password")
				};

				SecStatusCode res;
				var match = SecKeyChain.QueryAsRecord (rec, out res);
				if (res == SecStatusCode.Success)
				{
					string accountInfo = match.Account.ToString();
					if(!string.IsNullOrWhiteSpace(accountInfo) && accountInfo.IndexOf((char)3) != -1)
					{
						string[] accountInfos = accountInfo.Split((char)3);
						if(accountInfos != null && accountInfos.Length > 1)
						{
							userId = accountInfos[0];
							userName = accountInfos[1];
							password = match.ValueData.ToString();
							result = !string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password);
						}
					}
				}
			}
			catch
			{
			}

			return result;
		}
	}
}

