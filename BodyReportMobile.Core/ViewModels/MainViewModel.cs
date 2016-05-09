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
using Xamarin.Forms;
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
        
        protected override async void Show()
        {
            try
            {
                base.Show();

                AppTools.Instance.Init();

                LanguageViewModel.ReloadApplicationLanguage();
                InitTranslation(); //Reload for language

                await ManageUserConnection();

                await SynchronizeWebData();
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

        private async Task ManageUserConnection()
        {
            ActionIsInProgress = true;

            try
            {
                if (LoginManager.Instance.Init())
                {
                    DisplayUserProfil();
                    await LoginManager.Instance.ConnectUser(false); // no need treat response, just for connect user
                }
                else
                {
                    await LoginViewModel.DisplayViewModel();
                    InitTranslation(); // Security if user change language
                }
            }
            catch //(Exception except)
            {
            }
            finally
            {
                ActionIsInProgress = false;
            }
        }

        private async Task SynchronizeWebData()
		{
            ActionIsInProgress = true;
            try
			{
                // download user image
                string localUserImagePath = GetUserImageLocalPath();
                string urlImage = string.Format("{0}images/userprofil/{1}.png", HttpConnector.Instance.BaseUrl, UserData.Instance.UserInfo.UserId);
                if (await HttpConnector.Instance.DownloadFile(urlImage, GetUserImageLocalPath()))
                    DisplayUserProfil();
                
                //Synchronise Web data to local database
                var muscleList = await MuscleWebService.FindMuscles();
                if(muscleList != null)
                {
				    var muscleManager = new MuscleManager(_dbContext);
				    muscleManager.UpdateMuscleList(muscleList);
                }

                var muscularGroupList = await MuscularGroupWebService.FindMuscularGroups();
                if (muscularGroupList != null)
                {
                    var muscularGroupManager = new MuscularGroupManager(_dbContext);
                    muscularGroupManager.UpdateMuscularGroupList(muscularGroupList);
                }

                var bodyExerciseList = await BodyExerciseWebService.FindBodyExercises();
                if (bodyExerciseList != null)
                {
                    var bodyExerciseManager = new BodyExerciseManager(_dbContext);
                    bodyExerciseManager.UpdateBodyExerciseList(bodyExerciseList);
                }

                var translationList = await TranslationWebService.FindTranslations();
                if (translationList != null)
                {
                    var translationManager = new TranslationManager(_dbContext);
                    translationManager.UpdateTranslationList(translationList);
                }
			}
			catch (Exception exception)
			{
				// TODO log
			}
            finally
            {
                ActionIsInProgress = false;
            }
		}

		public ICommand GoToTrainingJournalCommand
		{
			get
			{
				return new Command (async () => {
                    if (ActionIsInProgress)
                        return;

                    try
                    {
                        ActionIsInProgress = true;
                        var viewModel = new TrainingJournalViewModel();
                        await ShowModalViewModel(viewModel, this);
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

        /// <summary>
        /// Change language with user choice list view
        /// </summary>
		public ICommand GoToChangeLanguageCommand
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
                        {
                            InitTranslation();
                            LanguageViewModel.SaveApplicationLanguage();
                        }
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
	}
}


