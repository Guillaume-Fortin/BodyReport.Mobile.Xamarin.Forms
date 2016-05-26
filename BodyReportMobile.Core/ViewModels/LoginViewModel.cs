using System;
using BodyReportMobile.Core.ViewModels;
using System.Windows.Input;
using Message;
using BodyReportMobile.Core.Framework;
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

        public static async Task<bool> DisplayViewModelAsync(BaseViewModel parent = null)
        {
            var viewModel = new LoginViewModel();
            return await ShowModalViewModelAsync(viewModel, parent, false, true);
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

		private async Task LoginActionAsync()
        {
            if (await LogInUserAsync())
			{
				CloseViewModel();
			}
		}

        private async Task<bool> LogInUserAsync()
		{
            bool result = false;

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
                    result = await LoginManager.Instance.ConnectUserAsync(_userName, _password, false);
                    if(!result)
                        await userDialog.AlertAsync(Translation.Get(TRS.CONNECTION_FAILED), Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
                }
            }
            catch(Exception except)
            {
                await userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
            return result;
        }

        private async Task RegisterAccountActionAsync()
        {
            try
            {
                if (await RegisterAccountViewModel.DisplayViewModelAsync(this))
                    InformationsLabel = Translation.Get(TRS.VERIFY_SPAM_INTO_YOUR_MAIL_BOX);
            }
            catch (Exception except)
            {
                var userDialog = Resolver.Resolve<IUserDialogs>();
                await userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
        }

        /// <summary>
        /// Change language with user choice list view
        /// </summary>
		private async Task ChangeLanguageActionAsync()
        {
            try
            {
                if (await LanguageViewModel.DisplayChooseLanguageAsync(this))
                {
                    InitTranslation();
                    LanguageViewModel.SaveApplicationLanguage();
                }
            }
            catch
            {
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

        #region Command

        private ICommand _logInCommand = null;
        public ICommand LogInCommand
        {
            get
            {
                if (_logInCommand == null)
                {
                    _logInCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await LoginActionAsync();
                    });
                }

                return _logInCommand;
            }
        }

        private ICommand _registerAccountCommand = null;
        public ICommand RegisterAccountCommand
        {
            get
            {
                if (_registerAccountCommand == null)
                {
                    _registerAccountCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await RegisterAccountActionAsync();
                    });
                }

                return _registerAccountCommand;
            }
        }

        private ICommand _changeLanguageCommand = null;
        public ICommand ChangeLanguageCommand
        {
            get
            {
                if (_changeLanguageCommand == null)
                {
                    _changeLanguageCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await ChangeLanguageActionAsync();
                    });
                }

                return _changeLanguageCommand;
            }
        }

        #endregion
    }
}

