using System;
using BodyReportMobile.Core.ViewModels;
using System.Windows.Input;
using Message;
using BodyReportMobile.Core.Framework;
using Xamarin.Forms;
using System.Threading.Tasks;
using BodyReportMobile.Core.Manager;
using XLabs.Ioc;
using Acr.UserDialogs;
using BodyReportMobile.Core.Data;
using BodyReportMobile.Core.Message;

namespace BodyReportMobile.Core.ViewModels
{
	public class LoginViewModel : BaseViewModel
	{
        private string _loginBtnLabel;
        private string _userNameLabel;
        private string _passwordLabel;
        private string _logInLabel;
        private string _registerLabel;
        private string _informationsLabel;

        private string _userName = string.Empty;
        private string _password = string.Empty;

        private string _languageFlagImageSource;

        public string LanguageFlagImageSource
        {
            get { return _languageFlagImageSource; }
            set
            {
                if (value != _languageFlagImageSource)
                {
                    _languageFlagImageSource = value;
                    OnPropertyChanged();
                }
            }
        }

        public LoginViewModel () : base()
        {
           _allowCancelViewModel = false;
            ShowDelayInMs = 0;
        }

        public static async Task<bool> DisplayViewModel(BaseViewModel parent = null)
        {
            var viewModel = new LoginViewModel();
            return await ShowModalViewModel(viewModel, parent);
        }
        
        private string GeLanguageFlagImageSource(LangType langType)
        {
            return string.Format("flag_{0}.png", Translation.GetLangExt(langType)).Replace('-', '_');
        }

        protected override void InitTranslation()
		{
			base.InitTranslation ();

			TitleLabel = Translation.Get (TRS.CONNECTION);
			LoginBtnLabel = Translation.Get (TRS.LOG_IN);
            UserNameLabel = Translation.Get(TRS.USER_NAME);
            PasswordLabel = Translation.Get(TRS.PASSWORD);
            LogInLabel = Translation.Get(TRS.LOG_IN);
            RegisterLabel = Translation.Get(TRS.REGISTER);
            InformationsLabel = Translation.Get(TRS.USE_A_LOCAL_ACCOUNT_TO_LOG_IN);
            LanguageFlagImageSource = LanguageViewModel.GeLanguageFlagImageSource(Translation.CurrentLang);

            OnPropertyChanged(null);
        }

		public ICommand LogInCommand
		{
			get
			{
				return new Command (async () => {
                    if (ActionIsInProgress)
                        return;

                    if (await LogInUser())
					{
						CloseViewModel();
					}
				});
			}
		}

        public ICommand RegisterAccountCommand
        {
            get
            {
                return new Command(async () => {
                    if (ActionIsInProgress)
                        return;

                    if (await RegisterAccount())
                    {
                        CloseViewModel();
                    }
                });
            }
        }

        private async Task<bool> LogInUser()
		{
            bool result = false;
            ActionIsInProgress = true;

            var userDialog = Resolver.Resolve<IUserDialogs>();
            try
            {
                //Verify data for connect user
                if (string.IsNullOrEmpty(_userName) || string.IsNullOrEmpty(_password))
                {
                    string fieldName = string.IsNullOrEmpty(_userName) ? Translation.Get(TRS.USER_NAME) : Translation.Get(TRS.PASSWORD);
                    string message = string.Format(Translation.Get(TRS.THE_P0_FIELD_IS_REQUIRED), fieldName);
                    await userDialog.AlertAsync(message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
                }
                else
                {
                    result = await LoginManager.Instance.ConnectUser(_userName, _password, false);
                    if(!result)
                        await userDialog.AlertAsync(Translation.Get(TRS.CONNECTION_FAILED), Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
                }
            }
            catch(Exception except)
            {
                await userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
            finally
            {
                ActionIsInProgress = false;
            }
            return result;
        }

        private async Task<bool> RegisterAccount()
        {
            bool result = false;
            ActionIsInProgress = true;
            try
            {
                if (await RegisterAccountViewModel.DisplayViewModel(this))
                    InformationsLabel = Translation.Get(TRS.VERIFY_SPAM_INTO_YOUR_MAIL_BOX);
            }
            catch (Exception except)
            {
                var userDialog = Resolver.Resolve<IUserDialogs>();
                await userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
            finally
            {
                ActionIsInProgress = false;
            }
            return result;
        }

        /// <summary>
        /// Change language with user choice list view
        /// </summary>
		public ICommand ChangeLanguageCommand
        {
            get
            {
                return new Command(async () => {

                    if (ActionIsInProgress)
                        return;

                    try
                    {
                        ActionIsInProgress = true;
                        if (await LanguageViewModel.DisplayChooseLanguage(this))
                            InitTranslation();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        ActionIsInProgress = false;
                    }
                });
            }
        }

        #region label properties binding

        public string LoginBtnLabel
        {
            get
            {
                return _loginBtnLabel;
            }
            set
            {
                _loginBtnLabel = value;
                OnPropertyChanged();
            }
        }

        public string UserNameLabel
        {
            get
            {
                return _userNameLabel;
            }
            set
            {
                _userNameLabel = value;
                OnPropertyChanged();
            }
        }

        public string PasswordLabel
        {
            get
            {
                return _passwordLabel;
            }
            set
            {
                _passwordLabel = value;
                OnPropertyChanged();
            }
        }

        public string LogInLabel
        {
            get
            {
                return _logInLabel;
            }
            set
            {
                _logInLabel = value;
                OnPropertyChanged();
            }
        }

        public string RegisterLabel
        {
            get
            {
                return _registerLabel;
            }
            set
            {
                _registerLabel = value;
                OnPropertyChanged();
            }
        }

        public string InformationsLabel
        {
            get
            {
                return _informationsLabel;
            }
            set
            {
                _informationsLabel = value;
                OnPropertyChanged();
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public int UserNameMaxLength { get; set; } = FieldsLength.UserName.Max;
        public int PasswordMaxLength { get; set; } = FieldsLength.Password.Max;

        #endregion
    }
}

