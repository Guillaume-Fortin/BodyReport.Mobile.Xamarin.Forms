using BodyReportMobile.Core.Data;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Manager;
using BodyReport.Message;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using XLabs.Ioc;
using Acr.UserDialogs;

namespace BodyReportMobile.Core.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        public MenuViewModel() : base()
        {
        }

        protected override void InitTranslation()
        {
            base.InitTranslation();

            TitleLabel = Translation.Get(TRS.MENU);
            ConfigurationLabel = Translation.Get(TRS.CONFIGURATION);
            ChangeLanguageLabel = Translation.Get(TRS.LANGUAGE);
            UserLabel = Translation.Get(TRS.USER);
            EditUserProfileLabel = Translation.Get(TRS.ACCOUNT_INFORMATION);
            LogoffLabel = Translation.Get(TRS.LOG_OFF);
            ConfidentialityRulesLabel = Translation.Get(TRS.CONFIDENTIALITY_RULES);
            LanguageFlagImageSource = LanguageViewModel.GeLanguageFlagImageSource(Translation.CurrentLang);
        }

        public static async Task<bool> ShowAsync(BaseViewModel parent = null)
        {
            var viewModel = new MenuViewModel();
            return await ShowModalViewModelAsync(viewModel, parent);
        }

        private async Task EditUserProfileActionAsync()
        {
            try
            {
                await EditUserProfileViewModel.ShowAsync(UserData.Instance.UserInfo.UserId, true, this);
            }
            catch(Exception except)
            {
                ILogger.Instance.Error("Unable to edit user profile", except);
            }
        }

        private async Task LogOffActionAsync()
        {
            try
            {
                var userDialog = Resolver.Resolve<IUserDialogs>();
                if (await userDialog.ConfirmAsync(Translation.Get(TRS.ARE_YOU_SURE_YOU_WANT_TO_LOG_OFF_PI), Translation.Get(TRS.CONFIRMATION), Translation.Get(TRS.YES), Translation.Get(TRS.NO)))
                {
                    LoginManager.Instance.LogOff();
                    CloseViewModel();
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to log out user", except);
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
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to change language", except);
            }
        }

        private async Task DisplayConfidentialityRulesActionAsync()
        {
            try
            {
                await ConfidentialityRulesViewModel.ShowAsync(this);
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to display confidential rules", except);
            }
        }

        #region properties

        private string _configurationLabel;
        public string ConfigurationLabel
        {
            get { return _configurationLabel; }
            set
            {
                _configurationLabel = value;
                OnPropertyChanged();
            }
        }

        private string _changeLanguageLabel;
        public string ChangeLanguageLabel
        {
            get { return _changeLanguageLabel; }
            set
            {
                _changeLanguageLabel = value;
                OnPropertyChanged();
            }
        }

        private string _userLabel;
        public string UserLabel
        {
            get { return _userLabel; }
            set
            {
                _userLabel = value;
                OnPropertyChanged();
            }
        }

        private string _editUserProfileLabel;
        public string EditUserProfileLabel
        {
            get { return _editUserProfileLabel; }
            set
            {
                _editUserProfileLabel = value;
                OnPropertyChanged();
            }
        }

        private string _logoffLabel;
        public string LogoffLabel
        {
            get { return _logoffLabel; }
            set
            {
                _logoffLabel = value;
                OnPropertyChanged();
            }
        }

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

        private string _confidentialityRulesLabel;
        public string ConfidentialityRulesLabel
        {
            get { return _confidentialityRulesLabel; }
            set
            {
                _confidentialityRulesLabel = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        private ICommand _editUserProfileCommand = null;
        public ICommand EditUserProfileCommand
        {
            get
            {
                if (_editUserProfileCommand == null)
                {
                    _editUserProfileCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await EditUserProfileActionAsync();
                    });
                }

                return _editUserProfileCommand;
            }
        }

        private ICommand _logOffCommand = null;
        public ICommand LogOffCommand
        {
            get
            {
                if (_logOffCommand == null)
                {
                    _logOffCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await LogOffActionAsync();
                    });
                }

                return _logOffCommand;
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

        private ICommand _displayConfidentialityRulesCommand = null;
        public ICommand DisplayConfidentialityRulesCommand
        {
            get
            {
                if (_displayConfidentialityRulesCommand == null)
                {
                    _displayConfidentialityRulesCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await DisplayConfidentialityRulesActionAsync();
                    });
                }

                return _displayConfidentialityRulesCommand;
            }
        }

        #endregion
    }
}
