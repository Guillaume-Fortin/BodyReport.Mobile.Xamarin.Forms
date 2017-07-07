using System;
using System.Windows.Input;
using System.Threading.Tasks;
using BodyReport.Message;
using XLabs.Ioc;
using BodyReportMobile.Core.WebServices;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Data;
using System.IO;
using Acr.UserDialogs;
using Plugin.Media;
using Plugin.Media.Abstractions;
using BodyReportMobile.Core.Manager;
using Xamarin.Forms;

namespace BodyReportMobile.Core.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
        public string TrainingJournalLabel { get; set;}
        public string _userProfilImage { get; set; }

        public string UserProfilImage
        {
            get { return _userProfilImage; }
            set
            {
                _userProfilImage = value;
                OnPropertyChanged();
            }
        }

        private IFileManager _fileManager;
        private IUserDialogs _userDialog;

        public MainViewModel() : base()
        {
            _fileManager = Resolver.Resolve<IFileManager>();
            _userDialog = Resolver.Resolve<IUserDialogs>();
            ShowDelayInMs = 0;
			DisableBackButton = true;
        }

        public static async Task<bool> ShowAsync(BaseViewModel parent = null)
        {
            var viewModel = new MainViewModel();
            return await ShowModalViewModelAsync(viewModel, parent, true);
        }

        protected override async Task ShowAsync()
        {
            try
            {
                await base.ShowAsync();

                ActionIsInProgress = true;
                await InitUserConnectionAsync();
            }
            catch(Exception except)
            {
                await _userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
            finally
            {
                ActionIsInProgress = false;
            }
        }

        private async Task InitUserConnectionAsync()
        {
            try
            {
                await DataSyncViewModel.ShowViewAsync(this);

				InitTranslation ();
                DisplayUserProfil();
            }
            catch(Exception except)
            {
                ILogger.Instance.Error("Unable to Show DataSyncViewModel", except);
            }
        }

		protected override void InitTranslation()
		{
            base.InitTranslation();

			TitleLabel = "BodyReport";
            if(Device.RuntimePlatform == Device.Android)
				MenuLabel = Translation.Get (TRS.MENU);
            TrainingJournalLabel = Translation.Get (TRS.TRAINING_JOURNAL);

            OnPropertyChanged(null);
        }

		private string GeLanguageFlagImageSource(LangType langType)
		{
			return string.Format ("flag_{0}.png", Translation.GetLangExt (langType)).Replace('-', '_');
		}

        private void DisplayUserProfil()
        {
            string imagePath = UserData.Instance.UserInfo == null ? null : AppTools.Instance.GetUserImageLocalPath(UserData.Instance.UserInfo.UserId);

            if (!string.IsNullOrWhiteSpace(imagePath) && _fileManager.FileExist(imagePath))
                UserProfilImage = imagePath;
            else
                UserProfilImage = "logo.png";

            UserNameLabel = LoginManager.Instance.UserName;

            var userUnit = UserData.Instance.UserInfo?.Unit;
            string weightUnit = "kg", lengthUnit = "cm";
            if (userUnit == (int)TUnitType.Imperial)
            {
                weightUnit = Translation.Get(TRS.POUND);
                lengthUnit = Translation.Get(TRS.INCH);
            }
            UserHeightLabel = string.Format("{0} {1}", UserData.Instance.UserInfo?.Height, lengthUnit);
            UserWeightLabel = string.Format("{0} {1}", UserData.Instance.UserInfo?.Weight, weightUnit);
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

        private async Task MenuActionAsync()
        {
            var oldLang = Translation.CurrentLang;
            await MenuViewModel.ShowAsync(this);
            if (oldLang != Translation.CurrentLang)
                InitTranslation();
            if(string.IsNullOrWhiteSpace(UserData.Instance.UserInfo.UserId))
            {
                //user deconnection detected
                await InitUserConnectionAsync();
            }

            //Security refresh user info
            DisplayUserProfil();
        }
        
        private async Task SelectUserPictureActionAsync()
        {
            try
            {
                string imagePath = UserData.Instance.UserInfo == null ? null : AppTools.Instance.GetUserImageLocalPath(UserData.Instance.UserInfo.UserId);
                
                if (string.IsNullOrWhiteSpace(imagePath))
                    return;

                MediaFile mediaFile = null;
                await CrossMedia.Current.Initialize();

                if (await _userDialog.ConfirmAsync(Translation.Get(TRS.DO_YOU_WANT_TAKE_PHOTO_WITH_CAMERA_PI), Translation.Get(TRS.QUESTION), Translation.Get(TRS.YES), Translation.Get(TRS.NO)))
                {
                    // take a photo
                    if (!CrossMedia.Current.IsCameraAvailable)
                    {
                        await _userDialog.AlertAsync(string.Format(Translation.Get(TRS.P0_ISNT_SUPPORTED_ON_THIS_DEVICE), Translation.Get(TRS.TAKE_PHOTO)), Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
                        return;
                    }
                    StoreCameraMediaOptions cameraOption = new StoreCameraMediaOptions()
                    {
                        Directory = "BodyReport",
                        Name = "tempPhoto.jpg",
                        SaveToAlbum = true
                    };
                    mediaFile = await CrossMedia.Current.TakePhotoAsync(cameraOption);
                }
                else
                {
                    if (!CrossMedia.Current.IsTakePhotoSupported)
                    {
                        await _userDialog.AlertAsync(string.Format(Translation.Get(TRS.P0_ISNT_SUPPORTED_ON_THIS_DEVICE), Translation.Get(TRS.SELECT_PICTURE)), Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
                        return;
                    }
                    mediaFile = await CrossMedia.Current.PickPhotoAsync();
                }

                if (mediaFile == null || string.IsNullOrWhiteSpace(mediaFile.Path))
                    return;

                string pngImagePath = Path.Combine(AppTools.TempDirectory, UserData.Instance.UserInfo.UserId.ToString() + ".png");
                BodyReportMobile.Core.Framework.IMedia.Instance.CompressImageAsPng(mediaFile.Path, pngImagePath, 400);
                
                //Upload on server
                string uploadedRelativeUrl = await UserProfileWebService.UploadUserProfilePictureAsync(pngImagePath);

                if (string.IsNullOrWhiteSpace(uploadedRelativeUrl))
                {
                    _fileManager.DeleteFile(pngImagePath);
                    await _userDialog.AlertAsync(string.Format(Translation.Get(TRS.IMPOSSIBLE_TO_UPDATE_P0), Translation.Get(TRS.IMAGE)), Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
                }
                else //Copy file on local
                {
                    _fileManager.CopyFile(pngImagePath, imagePath);
                    _fileManager.DeleteFile(pngImagePath);
                    DisplayUserProfil();
                }
            }
            catch(Exception except)
            {
                ILogger.Instance.Error("Can't take user profile image", except);
            }
        }

        #region label properties binding

        private string _userNameLabel;
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

        private string _userHeightLabel;
        public string UserHeightLabel
        {
            get
            {
                return _userHeightLabel;
            }
            set
            {
                _userHeightLabel = value;
                OnPropertyChanged();
            }
        }

        private string _userWeightLabel;
        public string UserWeightLabel
        {
            get
            {
                return _userWeightLabel;
            }
            set
            {
                _userWeightLabel = value;
                OnPropertyChanged();
            }
        }

		private string _menuLabel;
		public string MenuLabel {
			get {
				return _menuLabel;
			}
			set {
				_menuLabel = value;
				OnPropertyChanged ();
			}
		}

        #endregion

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

        private ICommand _selectUserPictureCommand = null;
        public ICommand SelectUserPictureCommand
        {
            get
            {
                if (_selectUserPictureCommand == null)
                {
                    _selectUserPictureCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await SelectUserPictureActionAsync();
                    });
                }

                return _selectUserPictureCommand;
            }
        }

        private ICommand _menuCommand = null;
        public ICommand MenuCommand
        {
            get
            {
                if (_menuCommand == null)
                {
                    _menuCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await MenuActionAsync();
                    });
                }

                return _menuCommand;
            }
        }

        #endregion
    }
}


