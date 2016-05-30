using BodyReportMobile.Core.Data;
using BodyReportMobile.Core.Framework;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
