using BodyReportMobile.Core.Data;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Manager;
using BodyReport.Message;
using System;
using System.Threading.Tasks;
using XLabs.Ioc;

namespace BodyReportMobile.Core.ViewModels
{
    public class DataSyncViewModel : BaseViewModel
    {
        private ApplicationDbContext _dbContext;
        private double _maxSynchronizeCount = 6;
        private double _synchronizeCount = 1;

        public DataSyncViewModel() : base ()
		{
            _allowCancelViewModel = false;
            ShowDelayInMs = 0;
			DisableBackButton = true;
            DbContext = Resolver.Resolve<ISQLite>().GetConnection();
        }

        protected override void InitTranslation()
        {
            base.InitTranslation();

            TitleLabel = Translation.Get(TRS.SYNCHRONIZATION);
            ProgressionLabel = Translation.Get(TRS.PROGRESSION);
        }

        protected override async Task ShowAsync()
        {
            await base.ShowAsync();

            try
            {
                AppTools.Instance.Init();

                //Migrate table
                BodyReportMobile.Core.Crud.Module.Crud.MigrateTable(DbContext);

                LanguageViewModel.ReloadApplicationLanguage();
                InitTranslation(); //Reload for language

                PrimaryProgress = 0;
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
                
                var baseUrl = "http://www.bodyreport.org:5000/"; // need dns for ipv6 compatibility
                if (HttpConnector.Instance.BaseUrl != baseUrl)
                    HttpConnector.Instance.BaseUrl = baseUrl;

                await ManageUserConnectionAsync();

                Task.Run(async () =>
                {
                    //BodyReportMobile.Core.Crud.Module.Crud.TestRework(DbContext); // for test
                    await SynchronizeWebDataAsync();
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        CloseViewModel();
                    });                    
                }).Start();
                
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable SynchronizeData", except);
            }
            finally
            {
                ActionIsInProgress = false;
            }            
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
            PrimaryProgress = currentCount == 0 ? 0 : Math.Min(1, currentCount / maxCount);
        }
        

        private async Task SynchronizeBodyBuildingDatas()
        {
            await DataSync.SynchronizeMusclesAsync(DbContext);
            await DataSync.SynchronizeMuscularGroupAsync(DbContext);
            await DataSync.SynchronizeBodyExercisesAsync(DbContext);
        }
        

        private async Task SynchronizeWebDataAsync()
        {
            try
            {
                //Synchronise Web data to local database
                SynchronizationLabel = Translation.Get(TRS.TRANSLATIONS);
                await DataSync.SynchronizeTranslationsAsync(DbContext);
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;

                SynchronizationLabel = Translation.Get(TRS.COUNTRY);
                await DataSync.SynchronizeCountriesAsync();
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;

                SynchronizationLabel = Translation.Get(TRS.USER);
                await DataSync.SynchronizeUserImageAsync();
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;

                SynchronizationLabel = Translation.Get(TRS.COUNTRY);
                await DataSync.SynchronizeCountriesAsync();
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;

                SynchronizationLabel = Translation.Get(TRS.BODY_EXERCISES);
                await SynchronizeBodyBuildingDatas();
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;

                SynchronizationLabel = Translation.Get(TRS.TRAINING_JOURNAL);

                IProgress<double> progress = new Progress<double>(value => { SecondaryProgress = value; });
                await DataSync.SynchronizeTrainingWeeksAsync(DbContext, progress);
                SynchronizeProgress(_maxSynchronizeCount, _synchronizeCount);
                _synchronizeCount++;
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

        private double _primaryProgress = 0;
        public double PrimaryProgress
        {
            get
            {
                return _primaryProgress;
            }
            set
            {
                _primaryProgress = value;
                OnPropertyChanged();
            }
        }

        private double _secondaryProgress = 0;
        public double SecondaryProgress
        {
            get
            {
                return _secondaryProgress;
            }
            set
            {
                _secondaryProgress = value;
                OnPropertyChanged();
            }
        }

        private string _progressionLabel = "";
        public string ProgressionLabel
        {
            get
            {
                return _progressionLabel;
            }
            set
            {
                _progressionLabel = value;
                OnPropertyChanged();
            }
        }

        public ApplicationDbContext DbContext { get => _dbContext; set => _dbContext = value; }

        #endregion

    }
}
