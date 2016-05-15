using System;
using System.Windows.Input;
using System.Threading.Tasks;
using BodyReportMobile.Core.Message;
using System.Collections.Generic;
using Message;
using BodyReportMobile.Core.ServiceManagers;
using SQLite.Net;
using XLabs.Ioc;
using BodyReportMobile.Core.ViewModels.Generic;
using BodyReportMobile.Core.Manager;
using BodyReportMobile.Core.WebServices;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Data;
using System.IO;
using Acr.UserDialogs;

namespace BodyReportMobile.Core.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		public string MenuLabel { get; set;}
		public string ConfigurationLabel { get; set;}
		public string TrainingJournalLabel { get; set;}
		public string ChangeLanguageLabel { get; set;}
        public string _userProfilImage { get; set; }


        public string UserProfilImage
        {
            get { return _userProfilImage; }
            set
            {
                if (value != _userProfilImage)
                {
                    _userProfilImage = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _languageFlagImageSource;

		public string LanguageFlagImageSource
		{
			get { return _languageFlagImageSource; }
			set 
			{
				if (value != _languageFlagImageSource) {
					_languageFlagImageSource = value;
                    OnPropertyChanged();
				}
			}
		}

		private SQLiteConnection _dbContext;
        private IFileManager _fileManager;
        private string _userProfilLocalPath;

        public MainViewModel() : base()
        {
			_dbContext = Resolver.Resolve<ISQLite> ().GetConnection ();
            _fileManager = Resolver.Resolve<IFileManager>();
            ShowDelayInMs = 0;

            _userProfilLocalPath = Path.Combine(_fileManager.GetDocumentPath(), "userprofil");
            if (!_fileManager.DirectoryExist(_userProfilLocalPath))
                _fileManager.CreateDirectory(_userProfilLocalPath);
        }
        
        protected override async Task ShowAsync()
        {
            try
            {
                await base.ShowAsync();

                try
                {
                    ActionIsInProgress = true;

                    AppTools.Instance.Init();

                    LanguageViewModel.ReloadApplicationLanguage();
                    InitTranslation(); //Reload for language
                    
                    await ManageUserConnectionAsync();

                    await SynchronizeWebDataAsync();
                }
                catch(Exception except)
                {
                    ILogger.Instance.Error("Unable to Show MainViewModel", except);
                }
                finally
                {
                    ActionIsInProgress = false;
                }
            }
            catch(Exception except)
            {
                var userDialog = Resolver.Resolve<IUserDialogs>();
                await userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
        }

		protected override void InitTranslation()
		{
            base.InitTranslation();

			TitleLabel = "BodyReport";
			MenuLabel = Translation.Get (TRS.MENU);
			ConfigurationLabel = Translation.Get (TRS.CONFIGURATION);
			TrainingJournalLabel = Translation.Get (TRS.TRAINING_JOURNAL);
			ChangeLanguageLabel = Translation.Get (TRS.LANGUAGE);
			LanguageFlagImageSource = LanguageViewModel.GeLanguageFlagImageSource (Translation.CurrentLang);

            OnPropertyChanged(null);
        }

		private string GeLanguageFlagImageSource(LangType langType)
		{
			return string.Format ("flag_{0}.png", Translation.GetLangExt (langType)).Replace('-', '_');
		}

        private string GetUserImageLocalPath()
        {
            if (string.IsNullOrWhiteSpace(UserData.Instance.UserInfo.UserId))
                return null;
            return Path.Combine(_userProfilLocalPath, UserData.Instance.UserInfo.UserId + ".png");
        }

        private void DisplayUserProfil()
        {
            UserProfilImage = GetUserImageLocalPath();
        }

        private async Task ManageUserConnectionAsync()
        {
            try
            {
                if (LoginManager.Instance.Init())
                {
                    DisplayUserProfil();
                    await LoginManager.Instance.ConnectUserAsync(false); // no need treat response, just for connect user
                }
                else
                {
                    await LoginViewModel.DisplayViewModelAsync();
                    InitTranslation(); // Security if user change language
                }
            }
            catch //(Exception except)
            {
            }
        }

        private async Task SynchronizeWebDataAsync()
		{
            try
			{
                // download user image
                string localUserImagePath = GetUserImageLocalPath();
                string urlImage = string.Format("{0}images/userprofil/{1}.png", HttpConnector.Instance.BaseUrl, UserData.Instance.UserInfo.UserId);
                if (await HttpConnector.Instance.DownloadFileAsync(urlImage, GetUserImageLocalPath()))
                    DisplayUserProfil();
                
                //Synchronise Web data to local database
                var muscleList = await MuscleWebService.FindMusclesAsync();
                if(muscleList != null)
                {
				    var muscleManager = new MuscleManager(_dbContext);
				    muscleManager.UpdateMuscleList(muscleList);
                }

                var muscularGroupList = await MuscularGroupWebService.FindMuscularGroupsAsync();
                if (muscularGroupList != null)
                {
                    var muscularGroupManager = new MuscularGroupManager(_dbContext);
                    muscularGroupManager.UpdateMuscularGroupList(muscularGroupList);
                }

                var bodyExerciseList = await BodyExerciseWebService.FindBodyExercisesAsync();
                if (bodyExerciseList != null)
                {
                    var bodyExerciseManager = new BodyExerciseManager(_dbContext);
                    bodyExerciseManager.UpdateBodyExerciseList(bodyExerciseList);
                }

                var translationList = await TranslationWebService.FindTranslationsAsync();
                if (translationList != null)
                {
                    var translationManager = new TranslationManager(_dbContext);
                    translationManager.UpdateTranslationList(translationList);
                }
			}
			catch (Exception except)
			{
                ILogger.Instance.Error("Unable to synchronize web data for MainViewModel", except);
            }
		}

        private async Task GoToTrainingJournalActionAsync()
		{
            try
            {
                var viewModel = new TrainingJournalViewModel();
                await ShowModalViewModelAsync(viewModel, this);
            }
            catch
            {
            }
		}

        /// <summary>
        /// Change language with user choice list view
        /// </summary>
		private async Task GoToChangeLanguageActionAsync()
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
        
        #region Command

        private ICommand _goToTrainingJournalCommand = null;
        public ICommand GoToTrainingJournalCommand
        {
            get
            {
                if (_goToTrainingJournalCommand == null)
                {
                    _goToTrainingJournalCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await GoToTrainingJournalActionAsync();
                    });
                }

                return _goToTrainingJournalCommand;
            }
        }

        private ICommand _goToChangeLanguageCommand = null;
        public ICommand GoToChangeLanguageCommand
        {
            get
            {
                if (_goToChangeLanguageCommand == null)
                {
                    _goToChangeLanguageCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await GoToChangeLanguageActionAsync();
                    });
                }

                return _goToChangeLanguageCommand;
            }
        }

        #endregion
    }
}


