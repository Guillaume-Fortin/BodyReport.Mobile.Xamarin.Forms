using System;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;
using Acr.UserDialogs;
using Message;
using System.Threading.Tasks;

namespace BodyReportMobile.Core
{
	public class LoginManager
	{
		private ISecurity _security;
		private bool _busy = false;
		private static LoginManager _instance = null;
		/// <summary>
		/// The mvx messenger token.
		/// </summary>
		private readonly MvxSubscriptionToken _mvxMessengerLoginEntry;

		private string _userName = string.Empty;
		private string _password = string.Empty;

		private LoginManager ()
		{
			_security = Mvx.Resolve<ISecurity> ();
			var messenger = Mvx.Resolve<IMvxMessenger>();
			_mvxMessengerLoginEntry = messenger.Subscribe<MvxMessageLoginEntry>(OnLoginEntry);
			if (_mvxMessengerLoginEntry == null) // supress unused Warning
				_mvxMessengerLoginEntry = null;
		}

		public static LoginManager Instance {
			get {
				if(_instance == null)
					_instance = new LoginManager ();
				return _instance;
			}
		}

		public async Task Init()
		{
			try
			{
				_busy = true;
				bool result;
				if(_security.GetUserInfo (out _userName, out _password))
					result = await HttpConnector.Instance.ConnectUser(_userName, _password);
			}
			catch
			{
			}
			finally
			{
				_busy = false;
			}
		}

		private async void OnLoginEntry(MvxMessageLoginEntry message)
		{
			if (message == null || _busy)
				return;

			try
			{
				_busy = true;
				LoginConfig.DefaultCancelText = Translation.Get(TRS.CANCEL);
				LoginConfig.DefaultOkText = Translation.Get(TRS.OK);
				LoginConfig.DefaultTitle = Translation.Get(TRS.LOG_IN);
				LoginConfig.DefaultLoginPlaceholder = Translation.Get(TRS.USER_NAME);
				LoginConfig.DefaultPasswordPlaceholder = Translation.Get(TRS.PASSWORD);
				bool connectionOK = false;
				LoginResult loginResult;
				do
				{
					loginResult = await UserDialogs.Instance.LoginAsync(new LoginConfig
					{
						LoginValue = _userName,
						Message = Translation.Get(TRS.USE_A_LOCAL_ACCOUNT_TO_LOG_IN)
					});

					if(loginResult.Ok)
					{
						_userName = loginResult.LoginText;
						_password = loginResult.Password;
						if(!string.IsNullOrWhiteSpace(loginResult.LoginText) && !string.IsNullOrWhiteSpace(_password))
						{
							connectionOK = await HttpConnector.Instance.ConnectUser(_userName, _password);
							if(connectionOK)
								_security.SaveUserInfo(_userName, _password);
						}
					}
				}
				while(loginResult != null && loginResult.Ok && !connectionOK);
			}
			catch (Exception exception)
			{
				//TODO LOG
			}
			finally
			{
				_busy = false;
			}
		}
	}
}

