using System;
using Acr.UserDialogs;
using BodyReport.Message;
using System.Threading.Tasks;
using XLabs.Ioc;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.MvxMessages;
using BodyReportMobile.Core.WebServices;
using BodyReportMobile.Core.ServiceManagers;
using BodyReportMobile.Core.Data;

namespace BodyReportMobile.Core.Manager
{
	public class LoginManager
	{
		private ISecurity _security;
		private bool _busy = false;
		private static LoginManager _instance = null;

        private string _userId = string.Empty;
        private string _userName = string.Empty;
		private string _password = string.Empty;

		private LoginManager ()
		{
			_security = Resolver.Resolve<ISecurity> ();
            AppMessenger.AppInstance.Register<MvxMessageLoginEntry>(this, OnLoginEntryAsync);
            //TODO unscribe
		}

		public static LoginManager Instance {
			get {
				if(_instance == null)
					_instance = new LoginManager ();
				return _instance;
			}
		}

        private async Task<bool> RetreiveOnlineUserInfoAsync()
        {
            bool result = false;
            try
            {
                var userInfoKey = new UserInfoKey(); // no need userId here
                var userInfo = await UserInfoWebService.GetUserInfoAsync(userInfoKey);
                if (userInfo != null)
                {
                    var dbContext = Resolver.Resolve<ISQLite>().GetConnection();
                    var userInfoManager = new UserInfoManager(dbContext);
                    var userInfoList = userInfoManager.FindUserInfos();
                    if(userInfoList != null && userInfoList.Count > 0)
                    { // delete old userInfo
                        foreach (var userInfoDeleted in userInfoList)
                            userInfoManager.DeleteUserInfo(userInfoDeleted);
                    }
                    userInfoManager.UpdateUserInfo(userInfo);
                    UserData.Instance.UserInfo = userInfo;
                    _userId = userInfo.UserId;
                    result = true;
                }
            }
            catch
            {

            }
            return result;
        }

        private bool SaveEncryptedAccountData()
        {
            bool result = false;
            try
            {
                _security.SaveUserInfo(_userId, _userName, _password);
                return true;
            }
            catch
            {
            }
            return result;
        }

        private bool LoadLocalUserInfo()
        {
            // load local login info
            var dbContext = Resolver.Resolve<ISQLite>().GetConnection();
            var userInfoManager = new UserInfoManager(dbContext);
            var userInfo = userInfoManager.GetUserInfo(new UserInfoKey() { UserId = _userId });
            if (userInfo != null)
            {
                UserData.Instance.UserInfo = userInfo;
                return true;
            }
            return false;
        }

        public bool Init()
        {
            bool result = false;
            try
            {
                string userId, userName, password;
                if (_security.GetUserInfo(out userId, out userName, out password))
                {
                    _userId = userId;
                    _userName = userName;
                    _password = password;
                    result = LoadLocalUserInfo();
                }
            }
            catch
            {
            }
            return result;
        }

        public async Task<bool> ConnectUserAsync(bool autoPromptLogin)
        {
            return await ConnectUserAsync(_userName, _password, autoPromptLogin);
        }

        public async Task<bool> ConnectUserAsync(string userName, string password, bool autoPromptLogin)
        {
            bool result = false;
            try
            {
                bool userConnected = await HttpConnector.Instance.ConnectUserAsync(userName, password, Translation.CurrentLang, autoPromptLogin);
                if (userConnected) // web login
                {
                    _userId = string.Empty;
                    _userName = userName;
                    _password = password;
                    if (await RetreiveOnlineUserInfoAsync())
                    {
                        result = SaveEncryptedAccountData();
                    }
                }
			}
			catch(Exception exception)
			{
                throw exception;
			}
			finally
			{
			}
            return result;
		}

        private async void OnLoginEntryAsync(MvxMessageLoginEntry message)
        {
            if (message == null)
                return;

            await PromptLoginAsync();
        }

        public async Task<bool> PromptLoginAsync(bool allowCancel=false)
        {
			if (_busy)
				return false;

            bool connectionOK = false;
            try
			{
				_busy = true;
				LoginConfig.DefaultCancelText = allowCancel ? Translation.Get(TRS.CANCEL) : null;
				LoginConfig.DefaultOkText = Translation.Get(TRS.OK);
				LoginConfig.DefaultTitle = Translation.Get(TRS.LOG_IN);
				LoginConfig.DefaultLoginPlaceholder = Translation.Get(TRS.USER_NAME);
				LoginConfig.DefaultPasswordPlaceholder = Translation.Get(TRS.PASSWORD);
				LoginResult loginResult;
				do
				{
					loginResult = await UserDialogs.Instance.LoginAsync(new LoginConfig
					{
						LoginValue = _userName,
						Message = Translation.Get(TRS.USE_A_LOCAL_ACCOUNT_TO_LOG_IN),
					});

					if(loginResult.Ok)
					{
                        string userName = loginResult.LoginText;
                        string password = loginResult.Password;
						if(!string.IsNullOrWhiteSpace(loginResult.LoginText) && !string.IsNullOrWhiteSpace(_password))
						{
							if(await ConnectUserAsync(userName, password, true))
                                connectionOK = SaveEncryptedAccountData();
                        }
					}
				}
				while(loginResult != null && loginResult.Ok && !connectionOK);
			}
			catch //(Exception exception)
			{
				//TODO LOG
			}
			finally
			{
				_busy = false;
			}
            return connectionOK;

        }
	}
}

