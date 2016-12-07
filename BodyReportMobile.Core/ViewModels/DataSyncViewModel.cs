using BodyReportMobile.Core.Data;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Manager;
using BodyReport.Message;
using SQLite.Net;
using System;
using System.Threading.Tasks;
using XLabs.Ioc;

namespace BodyReportMobile.Core.ViewModels
{
    public class DataSyncViewModel : BaseViewModel
    {
        private SQLiteConnection _dbContext;
        private double _maxSynchronizeCount = 6;
        private double _synchronizeCount = 1;

        public DataSyncViewModel() : base ()
		{
            _allowCancelViewModel = false;
            ShowDelayInMs = 0;
			DisableBackButton = true;
            _dbContext = Resolver.Resolve<ISQLite>().GetConnection();
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
                await SynchronizeDataAsync();
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

        public static async Task<bool> ShowViewAsync(BaseViewModel parent = null)
        {
            var viewModel = new DataSyncViewModel();
            return await ShowModalViewModelAsync(viewModel, parent, false);
        }

        private async Task SynchronizeDataAsync()
        {
            try
            {
                ActionIsInProgress = true;
                
                var baseUrl = "http://163.172.13.105:5000/"; // prefer use ip of www.bodyreport.org
                if (HttpConnector.Instance.BaseUrl != baseUrl)
                    HttpConnector.Instance.BaseUrl = baseUrl;

                await ManageUserConnectionAsync();

                await SynchronizeWebDataAsync();
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

        

        private async Task SynchronizeWebDataAsync()
        {
            try
            {
                //Synchronise Web data to local database
                var userImageTask = DataSync.SynchronizeUserImageAsync();
                var countriesTask = DataSync.SynchronizeCountriesAsync();
                var musclesTask = DataSync.SynchronizeMusclesAsync(_dbContext);
                var muscularGroupTask = DataSync.SynchronizeMuscularGroupAsync(_dbContext);
                var bodyExercisesTask = DataSync.SynchronizeBodyExercisesAsync(_dbContext);
                var translationsTask = DataSync.SynchronizeTranslationsAsync(_dbContext);
                var trainingWeeksTask = DataSync.SynchronizeTrainingWeeksAsync(_dbContext);

                //Wait end of end of synchronisation
                SynchronizationLabel = Translation.Get(TRS.USER);
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;
                await userImageTask;
                
                SynchronizationLabel = Translation.Get(TRS.COUNTRY);
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;
                await countriesTask;
                
                SynchronizationLabel = Translation.Get(TRS.MUSCLES);
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;
                await musclesTask;
                
                SynchronizationLabel = Translation.Get(TRS.MUSCULAR_GROUP);
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;
                await muscularGroupTask;

                SynchronizationLabel = Translation.Get(TRS.BODY_EXERCISES);
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;
                await bodyExercisesTask;

                SynchronizationLabel = Translation.Get(TRS.TRANSLATIONS);
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;
                await translationsTask;
                
                SynchronizationLabel = Translation.Get(TRS.TRAINING_WEEK);
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;
                await trainingWeeksTask;
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
