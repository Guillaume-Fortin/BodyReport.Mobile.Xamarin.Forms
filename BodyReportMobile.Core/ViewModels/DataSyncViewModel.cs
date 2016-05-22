using BodyReportMobile.Core.Data;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Manager;
using BodyReportMobile.Core.ServiceManagers;
using BodyReportMobile.Core.WebServices;
using Message;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLabs.Ioc;

namespace BodyReportMobile.Core.ViewModels
{
    public class DataSyncViewModel : BaseViewModel
    {
        private SQLiteConnection _dbContext;
        private IFileManager _fileManager;

        public DataSyncViewModel() : base ()
		{
            _allowCancelViewModel = false;
            ShowDelayInMs = 0;
            _dbContext = Resolver.Resolve<ISQLite>().GetConnection();
            _fileManager = Resolver.Resolve<IFileManager>();
        }

        protected override void InitTranslation()
        {
            base.InitTranslation();

            TitleLabel = Translation.Get(TRS.SYNCHRONIZATION);
            SynchronizationLabel = Translation.Get(TRS.SYNCHRONIZE_DATAS);
        }

        protected override async Task ShowAsync()
        {
            await base.ShowAsync();

            try
            {
                AppTools.Instance.Init();

                LanguageViewModel.ReloadApplicationLanguage();
                InitTranslation(); //Reload for language

                SynchronizationProgress = 0;
                await SynchronizeData();
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to terminate ShowAsync", except);
            }
            finally
            {
                ActionIsInProgress = false;
            }
        }

        public static async Task<bool> ShowAsync(BaseViewModel parent = null)
        {
            var viewModel = new DataSyncViewModel();
            return await ShowModalViewModelAsync(viewModel, parent);
        }

        private async Task SynchronizeData()
        {
            try
            {
                ActionIsInProgress = true;
                await ManageUserConnectionAsync();

                await SynchronizeWebData();
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable SynchronizeData", except);
            }
            finally
            {
                ActionIsInProgress = false;
            }
            CloseViewModel();
        }

        private async Task ManageUserConnectionAsync()
        {
            try
            {
                if (LoginManager.Instance.Init())
                {
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

        private void SynchronizeProgress(double maxCount, double currentCount)
        {
            maxCount = Math.Max(maxCount, currentCount);
            SynchronizationProgress = currentCount == 0 ? 0 : Math.Min(1, currentCount / maxCount);
        }

        private async Task SynchronizeWebData()
        {
            try
            {
                double maxSynchronizeCount = 5;
                double synchronizeCount = 1;
                // download user image
                SynchronizationLabel = string.Format("{0} : {1}", Translation.Get(TRS.SYNCHRONIZE_DATAS), Translation.Get(TRS.IMAGE));
                SynchronizeProgress(maxSynchronizeCount, synchronizeCount);
                synchronizeCount++;
                string localUserImagePath = UserData.Instance.UserInfo == null ? null : AppTools.Instance.GetUserImageLocalPath(UserData.Instance.UserInfo.UserId);

                if (!string.IsNullOrWhiteSpace(localUserImagePath))
                {
                    string urlImage = await UserProfileWebService.GetUserProfileImageRelativeUrlAsync(UserData.Instance.UserInfo.UserId);
                    if (string.IsNullOrEmpty(urlImage))
                    {
                        if (_fileManager.FileExist(localUserImagePath))
                            _fileManager.DeleteFile(localUserImagePath);
                    }
                    else
                        await HttpConnector.Instance.DownloadFileAsync(urlImage, localUserImagePath);
                }

                //Synchronise Web data to local database
                SynchronizationLabel = string.Format("{0} : {1}", Translation.Get(TRS.SYNCHRONIZE_DATAS), Translation.Get(TRS.MUSCLES));
                SynchronizeProgress(maxSynchronizeCount, synchronizeCount);
                synchronizeCount++;
                var muscleList = await MuscleWebService.FindMusclesAsync();
                if (muscleList != null)
                {
                    var muscleManager = new MuscleManager(_dbContext);
                    muscleManager.UpdateMuscleList(muscleList);
                }

                //Synchronize Muscular groups
                SynchronizationLabel = string.Format("{0} : {1}", Translation.Get(TRS.SYNCHRONIZE_DATAS), Translation.Get(TRS.MUSCULAR_GROUP));
                SynchronizeProgress(maxSynchronizeCount, synchronizeCount);
                synchronizeCount++;
                var muscularGroupList = await MuscularGroupWebService.FindMuscularGroupsAsync();
                if (muscularGroupList != null)
                {
                    var muscularGroupManager = new MuscularGroupManager(_dbContext);
                    muscularGroupManager.UpdateMuscularGroupList(muscularGroupList);
                }

                //Synchronize body exercises
                SynchronizationLabel = string.Format("{0} : {1}", Translation.Get(TRS.SYNCHRONIZE_DATAS), Translation.Get(TRS.BODY_EXERCISES));
                SynchronizeProgress(maxSynchronizeCount, synchronizeCount);
                synchronizeCount++;
                var bodyExerciseList = await BodyExerciseWebService.FindBodyExercisesAsync();
                if (bodyExerciseList != null)
                {
                    var bodyExerciseManager = new BodyExerciseManager(_dbContext);
                    bodyExerciseManager.UpdateBodyExerciseList(bodyExerciseList);

                    //Synchronize body exercises images
                    maxSynchronizeCount += bodyExerciseList.Count;
                    SynchronizeProgress(maxSynchronizeCount, synchronizeCount);
                    SynchronizationLabel = string.Format("{0} : {1}", Translation.Get(TRS.SYNCHRONIZE_DATAS), Translation.Get(TRS.IMAGE));
                    List<Task> taskList = null;
                    string urlImage, localImagePath;
                    string urlImages = HttpConnector.Instance.BaseUrl + "images/bodyexercises/{0}";
                    foreach (var bodyExercise in bodyExerciseList)
                    {
                        if (taskList == null)
                            taskList = new List<Task>();
                        
                        if (string.IsNullOrWhiteSpace(bodyExercise.ImageName))
                            bodyExercise.ImageName = bodyExercise.Id.ToString() + ".png";
                        urlImage = string.Format(urlImages, bodyExercise.ImageName);
                        localImagePath = Path.Combine(AppTools.BodyExercisesImagesDirectory, bodyExercise.ImageName);
                        var t = AppTools.Instance.CachingImageAsync<BodyExercise>(bodyExercise, urlImage, localImagePath, null);
                        if (t != null)
                            taskList.Add(t);
                    }

                    if (taskList != null)
                    {
                        foreach (Task task in taskList)
                        {
                            await task;
                            SynchronizeProgress(maxSynchronizeCount, synchronizeCount);
                            synchronizeCount++;
                        }
                    }
                }


                //Synchronize Transalations
                SynchronizationLabel = string.Format("{0} : {1}", Translation.Get(TRS.SYNCHRONIZE_DATAS), Translation.Get(TRS.TRANSLATIONS));
                SynchronizeProgress(maxSynchronizeCount, synchronizeCount);
                synchronizeCount++;
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

        #region label properties binding

        private string _synchronizationLabel = string.Empty;
        public string SynchronizationLabel
        {
            get
            {
                return _synchronizationLabel;
            }
            set
            {
                _synchronizationLabel = value;
                OnPropertyChanged();
            }
        }

        private double _synchronizationProgress = 0;
        public double SynchronizationProgress
        {
            get
            {
                return _synchronizationProgress;
            }
            set
            {
                _synchronizationProgress = value;
                OnPropertyChanged();
            }
        }

        #endregion

    }
}
