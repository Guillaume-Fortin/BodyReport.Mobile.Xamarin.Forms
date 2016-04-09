using System;
using Message;

namespace BodyReportMobile.Core.Data
{
	public class UserData
	{
		private static UserData _instance = null;

		private UserData ()
		{
		}

		public static UserData Instance {
			get {
				if(_instance == null)
					_instance = new UserData ();
				return _instance;
			}
		}

		public UserInfo UserInfo{ get; set; } = new UserInfo();
	}
}

