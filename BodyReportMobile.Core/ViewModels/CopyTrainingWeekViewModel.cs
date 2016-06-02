using Acr.UserDialogs;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.WebServices;
using BodyReportMobile.Core.Services;
using BodyReport.Message;
using BodyReport.Message.WebApi;
using SQLite.Net;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using XLabs.Ioc;

namespace BodyReportMobile.Core.ViewModels
{
    public class CopyTrainingWeekViewModel : BaseViewModel
    {
        private SQLiteConnection _dbContext;
        private TrainingWeekService _trainingWeekService;

        private TrainingWeek _originTrainingWeek;
        public CopyTrainingWeek CopyTrainingWeek { get; set; }

        public string OriginTrainingWeekLabel { get; set; }
        public string NewTrainingWeekLabel { get; set; }
        public string YearLabel { get; set; }
        public string WeekNumberLabel { get; set; }
        public string ValidateLabel { get; set; }

        public CopyTrainingWeekViewModel() : base ()
		{
            _dbContext = Resolver.Resolve<ISQLite>().GetConnection();
            _trainingWeekService = new TrainingWeekService(_dbContext);
        }

        protected override void InitTranslation()
        {
            base.InitTranslation();

            TitleLabel = Translation.Get(TRS.TRAINING_WEEK) + ". " + Translation.Get(TRS.COPY);
            ValidateLabel = Translation.Get(TRS.VALIDATE);
            OriginTrainingWeekLabel = Translation.Get(TRS.ORIGIN_TRAINING_WEEK);
            NewTrainingWeekLabel = Translation.Get(TRS.NEW_TRAINING_WEEK);
            YearLabel = Translation.Get(TRS.YEAR);
            WeekNumberLabel = Translation.Get(TRS.WEEK_NUMBER);
            OnPropertyChanged(null);
        }

        protected override async Task ShowAsync()
        {
            await base.ShowAsync();
            SynchronizeData();
        }

        public static async Task<bool> ShowAsync(TrainingWeek originTrainingWeek, BaseViewModel parent = null)
        {
            var viewModel = new CopyTrainingWeekViewModel();
            viewModel._originTrainingWeek = originTrainingWeek;
            return await ShowModalViewModelAsync(viewModel, parent);
        }

        private void SynchronizeData()
        {
            CopyTrainingWeek = new CopyTrainingWeek();
            CopyTrainingWeek.UserId = _originTrainingWeek.UserId;
            CopyTrainingWeek.OriginYear = _originTrainingWeek.Year;
            CopyTrainingWeek.OriginWeekOfYear = _originTrainingWeek.WeekOfYear;
            CopyTrainingWeek.Year = _originTrainingWeek.Year;
            CopyTrainingWeek.WeekOfYear = Math.Min(52, _originTrainingWeek.WeekOfYear+1);
            
            OnPropertyChanged("CopyTrainingWeek");
        }

        private async Task ValidateActionAsync()
        {
            try
            {
                if (await ValidateFieldsAsync() && await SaveDataAsync())
                {
                    CloseViewModel();
                }
            }
            catch (Exception except)
            {
                var userDialog = Resolver.Resolve<IUserDialogs>();
                await userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
        }

        private async Task<bool> ValidateFieldsAsync()
        {
            var userDialog = Resolver.Resolve<IUserDialogs>();
            bool result = false;
            // check NewTrainingWeek not empty and NewTrainingWeek != OriginTrainingWeek
            if (CopyTrainingWeek != null && CopyTrainingWeek.Year > 0 && CopyTrainingWeek.WeekOfYear > 0 &&
               (CopyTrainingWeek.Year != CopyTrainingWeek.OriginYear || CopyTrainingWeek.WeekOfYear != CopyTrainingWeek.OriginWeekOfYear))
            {
                //Check new training doesn't exist
                var key = new TrainingWeekKey() { UserId = CopyTrainingWeek.UserId, Year = CopyTrainingWeek.Year, WeekOfYear = CopyTrainingWeek.WeekOfYear };
                var trainingWeek = await TrainingWeekWebService.GetTrainingWeekAsync(key);
                if(trainingWeek != null)
                {
                    await userDialog.AlertAsync(string.Format(Translation.Get(TRS.P0_ALREADY_EXIST), Translation.Get(TRS.TRAINING_WEEK)), Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
                    return false;
                }
                result = true;
            }
            else
            {
                await userDialog.AlertAsync(string.Format(Translation.Get(TRS.IMPOSSIBLE_TO_CREATE_P0), Translation.Get(TRS.TRAINING_WEEK)), Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
            return result;
        }

        private async Task<bool> SaveDataAsync()
        {
            bool result = false;
            TrainingWeek trainingWeek = await TrainingWeekWebService.CopyTrainingWeekAsync(CopyTrainingWeek);
            if(trainingWeek != null)
            {
                var trainingWeekScenario = new TrainingWeekScenario() { ManageTrainingDay = true };
                _trainingWeekService.UpdateTrainingWeek(trainingWeek, trainingWeekScenario);
                result = true;
            }
            return result;
        }

        #region Command

        private ICommand _validateCommand = null;
        public ICommand ValidateCommand
        {
            get
            {
                if (_validateCommand == null)
                {
                    _validateCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await ValidateActionAsync();
                    });
                }
                return _validateCommand;
            }
        }

        #endregion
    }
}
