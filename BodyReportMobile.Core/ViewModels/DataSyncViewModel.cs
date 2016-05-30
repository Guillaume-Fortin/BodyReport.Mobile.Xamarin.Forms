using BodyReportMobile.Core.Data;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Manager;
using BodyReportMobile.Core.ServiceManagers;
using BodyReportMobile.Core.WebServices;
using BodyReport.Message;
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
        private double _maxSynchronizeCount = 6;
        private double _synchronizeCount = 1;
        List<Task> _imagesSyncTaskList = null;

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
        }

        protected override async Task ShowAsync()
        {
            await base.ShowAsync();

            try
            {
                AppTools.Instance.Init();

                //Migrate table
                BodyReportMobile.Core.Crud.Module.Crud.MigrateTable(_dbContext);

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
            return await ShowModalViewModelAsync(viewModel, parent, false, true);
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
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to Manage user connection", except);
            }
        }

        private void SynchronizeProgress(double maxCount, double currentCount)
        {
            maxCount = Math.Max(maxCount, currentCount);
            SynchronizationProgress = currentCount == 0 ? 0 : Math.Min(1, currentCount / maxCount);
        }

        private async Task SynchronizeUserImageAsync()
        {
            try
            {
                // download user image
                SynchronizationLabel = Translation.Get(TRS.USER);
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;
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
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to synchronize user image", except);
            }
        }

        private async Task SynchronizeCountriesAsync()
        {
            try
            {
                //Synchronize Countries
                SynchronizationLabel = Translation.Get(TRS.COUNTRY);
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;
                var countryList = await CountryWebService.FindCountriesAsync();
                if (countryList != null)
                {
                    var countryManager = new CountryManager(_dbContext);
                    countryManager.UpdateCountryList(countryList);
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to synchronize countries", except);
            }
        }

        private async Task SynchronizeMusclesAsync()
        {
            try
            {
                //Synchronize Muscles
                SynchronizationLabel = Translation.Get(TRS.MUSCLES);
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;
                var muscleList = await MuscleWebService.FindMusclesAsync();
                if (muscleList != null)
                {
                    var muscleManager = new MuscleManager(_dbContext);
                    muscleManager.UpdateMuscleList(muscleList);
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to synchronize muscles", except);
            }
        }

        private async Task SynchronizeMuscularGroupAsync()
        {
            try
            {
                //Synchronize Muscular groups
                SynchronizationLabel = Translation.Get(TRS.MUSCULAR_GROUP);
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;
                var muscularGroupList = await MuscularGroupWebService.FindMuscularGroupsAsync();
                if (muscularGroupList != null)
                {
                    var muscularGroupManager = new MuscularGroupManager(_dbContext);
                    muscularGroupManager.UpdateMuscularGroupList(muscularGroupList);
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to synchronize mucular groups", except);
            }
        }

        private async Task SynchronizeBodyExercisesAsync()
        {
            try
            {
                //Synchronize body exercises
                SynchronizationLabel = Translation.Get(TRS.BODY_EXERCISES);
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;
                var bodyExerciseList = await BodyExerciseWebService.FindBodyExercisesAsync();
                if (bodyExerciseList != null)
                {
                    var bodyExerciseManager = new BodyExerciseManager(_dbContext);
                    bodyExerciseManager.UpdateBodyExerciseList(bodyExerciseList);

                    SynchronizationLabel = string.Format("{0} : {1}", Translation.Get(TRS.BODY_EXERCISES), Translation.Get(TRS.IMAGE));
                    //Synchronize body exercises images
                    string urlImage, localImagePath;
                    string urlImages = HttpConnector.Instance.BaseUrl + "images/bodyexercises/{0}";
                    foreach (var bodyExercise in bodyExerciseList)
                    {
                        if (_imagesSyncTaskList == null)
                            _imagesSyncTaskList = new List<Task>();

                        if (string.IsNullOrWhiteSpace(bodyExercise.ImageName))
                            bodyExercise.ImageName = bodyExercise.Id.ToString() + ".png";
                        urlImage = string.Format(urlImages, bodyExercise.ImageName);
                        localImagePath = Path.Combine(AppTools.BodyExercisesImagesDirectory, bodyExercise.ImageName);
                        var t = AppTools.Instance.CachingImageAsync<BodyExercise>(bodyExercise, urlImage, localImagePath, null);
                        if (t != null)
                            _imagesSyncTaskList.Add(t);
                    }
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to synchronize body exercises", except);
            }
        }

        private async Task SynchronizeTranslationsAsync()
        {
            try
            {
                //Synchronize Translations
                SynchronizationLabel = Translation.Get(TRS.TRANSLATIONS);
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;
                var translationList = await TranslationWebService.FindTranslationsAsync();
                if (translationList != null)
                {
                    var translationManager = new TranslationManager(_dbContext);
                    translationManager.UpdateTranslationList(translationList);
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to synchronize translations", except);
            }
        }

        private async Task SynchronizeTrainingWeeksAsync()
        {
            try
            {
                //Synchronize TrainingWeek with server (with trainingday and exercise)
                SynchronizationLabel = Translation.Get(TRS.TRAINING_WEEK);
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;
                var criteria = new TrainingWeekCriteria();
                criteria.UserId = new StringCriteria() { Equal = UserData.Instance.UserInfo.UserId };
                TrainingWeekScenario trainingWeekScenario = new TrainingWeekScenario() { ManageTrainingDay = false };
                var trainingWeekManager = new TrainingWeekManager(_dbContext);
                //retreive local data
                var localTrainingWeekList = trainingWeekManager.FindTrainingWeek(criteria, trainingWeekScenario);
                //retreive online data
                var criteriaList = new CriteriaList<TrainingWeekCriteria>() { criteria };
                var onlineTrainingWeekList = await TrainingWeekWebService.FindTrainingWeeksAsync(criteriaList, trainingWeekScenario);
                bool found;
                //Delete local data if not found on server
                if (localTrainingWeekList != null)
                {
                    var deletedTrainingWeekList = new List<TrainingWeek>();
                    foreach (var localTrainingWeek in localTrainingWeekList)
                    {
                        found = false;
                        if (onlineTrainingWeekList != null)
                        {
                            foreach (var onlineTrainingWeek in onlineTrainingWeekList)
                            {
                                if (TrainingWeek.IsEqualByKey(onlineTrainingWeek, localTrainingWeek))
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (!found)
                            deletedTrainingWeekList.Add(localTrainingWeek);
                    }
                    if (deletedTrainingWeekList.Count > 0)
                    {
                        _maxSynchronizeCount++;
                        SynchronizationLabel = Translation.Get(TRS.DELETE) + " :" + Translation.Get(TRS.TRAINING_WEEK);
                        SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                        _synchronizeCount++;
                        foreach (var deleteTrainingWeek in deletedTrainingWeekList)
                        {
                            //Delete in local database
                            trainingWeekManager.DeleteTrainingWeek(deleteTrainingWeek);
                            localTrainingWeekList.Remove(deleteTrainingWeek);
                        }
                    }
                }
                //if modification date online != local, get full trainingWeek online data and save them on local database
                var synchronizeTrainingWeekList = new List<TrainingWeek>();
                if (onlineTrainingWeekList != null)
                {
                    foreach (var onlineTrainingWeek in onlineTrainingWeekList)
                    {
                        found = false;
                        if (localTrainingWeekList != null)
                        {
                            foreach (var localTrainingWeek in localTrainingWeekList)
                            {
                                //Same trainingWeek
                                if (TrainingWeek.IsEqualByKey(onlineTrainingWeek, localTrainingWeek))
                                {
                                    if (onlineTrainingWeek.ModificationDate != null && localTrainingWeek.ModificationDate != null &&
                                        onlineTrainingWeek.ModificationDate.ToUniversalTime() != localTrainingWeek.ModificationDate.ToUniversalTime()) //ToUniversalTime for security...
                                        synchronizeTrainingWeekList.Add(onlineTrainingWeek);
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (!found)
                            synchronizeTrainingWeekList.Add(onlineTrainingWeek);
                    }
                }

                //Synchronize all trainingWeek data
                trainingWeekScenario = new TrainingWeekScenario()
                {
                    ManageTrainingDay = true,
                    TrainingDayScenario = new TrainingDayScenario() { ManageExercise = true }
                };
                if (synchronizeTrainingWeekList.Count > 0)
                {
                    _maxSynchronizeCount++;
                    SynchronizationLabel = Translation.Get(TRS.SEARCH) + " : " + Translation.Get(TRS.TRAINING_WEEK);
                    SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                    _synchronizeCount++;
                    criteriaList.Clear();
                    foreach (var trainingWeek in synchronizeTrainingWeekList)
                    {
                        criteria = new TrainingWeekCriteria();
                        criteria.UserId = new StringCriteria() { Equal = trainingWeek.UserId };
                        criteria.Year = new IntegerCriteria() { Equal = trainingWeek.Year };
                        criteria.WeekOfYear = new IntegerCriteria() { Equal = trainingWeek.WeekOfYear };
                        criteriaList.Add(criteria);
                    }
                    onlineTrainingWeekList = await TrainingWeekWebService.FindTrainingWeeksAsync(criteriaList, trainingWeekScenario);
                    if (onlineTrainingWeekList != null && onlineTrainingWeekList.Count > 0)
                    {
                        _maxSynchronizeCount += onlineTrainingWeekList.Count;
                        int count = 1;
                        foreach (var trainingWeek in onlineTrainingWeekList)
                        {
                            SynchronizationLabel = string.Format("{0} : {1}/{2}", Translation.Get(TRS.TRAINING_WEEK), count, onlineTrainingWeekList.Count);
                            SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                            _synchronizeCount++;
                            count++;
                            await Task.Delay(1); //Update UI
                            trainingWeekManager.UpdateTrainingWeek(trainingWeek, trainingWeekScenario);
                        }
                    }
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to synchronize training weeks", except);
            }
        }

        private async Task SynchronizeWebData()
        {
            try
            {
                //Synchronise Web data to local database
                await SynchronizeUserImageAsync();
                await SynchronizeCountriesAsync();
                await SynchronizeMusclesAsync();
                await SynchronizeMuscularGroupAsync();
                await SynchronizeBodyExercisesAsync();
                await SynchronizeTranslationsAsync();
                await SynchronizeTrainingWeeksAsync();
                
                //Wait end of image synchronisation
                if (_imagesSyncTaskList != null)
                {
                    SynchronizationLabel = Translation.Get(TRS.IMAGE);
                    _maxSynchronizeCount += _imagesSyncTaskList.Count;
                    SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                    foreach (Task task in _imagesSyncTaskList)
                    {
                        await task;
                        SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                        _synchronizeCount++;
                    }
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
