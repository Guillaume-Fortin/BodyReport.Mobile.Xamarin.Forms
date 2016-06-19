using Acr.UserDialogs;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Message.Binding;
using BodyReportMobile.Core.WebServices;
using BodyReport.Message;
using SQLite.Net;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using XLabs.Ioc;
using BodyReportMobile.Core.Services;

namespace BodyReportMobile.Core.ViewModels
{
    public class CreateTrainingDayViewModel : BaseViewModel
    {
        private SQLiteConnection _dbContext;
        private TrainingDayService _trainingDayService;
        private IUserDialogs _userDialog;
        private TEditMode _editMode;

        private TrainingDay _trainingDay;

        public CreateTrainingDayViewModel() : base()
        {
            ShowDelayInMs = 0;
            _dbContext = Resolver.Resolve<ISQLite>().GetConnection();
            _trainingDayService = new TrainingDayService(_dbContext);
            _userDialog = Resolver.Resolve<IUserDialogs>();
        }

        protected override async Task ShowAsync()
        {
            await base.ShowAsync();

            await SynchronizeDataAsync();
        }

        public static async Task<bool> ShowAsync(TrainingDay trainingDay, TEditMode editMode, BaseViewModel parent = null)
        {
            var viewModel = new CreateTrainingDayViewModel();
            viewModel._trainingDay = trainingDay;
            viewModel._editMode = editMode;
            return await ShowModalViewModelAsync(viewModel, parent);
        }

        protected override void InitTranslation()
        {
            base.InitTranslation();
            TitleLabel = Translation.Get(TRS.TRAINING_DAY);
            CreateTitle = _editMode == TEditMode.Create ? Translation.Get(TRS.CREATE) : Translation.Get(TRS.EDIT);
            YearLabel = Translation.Get(TRS.YEAR);
            WeekOfYearLabel = Translation.Get(TRS.WEEK_NUMBER);
            DayLabel = Translation.Get(TRS.DAY_OF_WEEK);
            ValidateLabel = Translation.Get(TRS.VALIDATE);
        }

        private async Task SynchronizeDataAsync()
        {
            try
            {
                BindingTrainingDay = new BindingTrainingDay()
                {
                    UserId = _trainingDay.UserId,
                    Year = _trainingDay.Year,
                    WeekOfYear = _trainingDay.WeekOfYear,
                    Day = _trainingDay.DayOfWeek,
                    DayLabel = Translation.Get(((DayOfWeek)_trainingDay.DayOfWeek).ToString().ToUpper()),
                    BeginTimeLabel = Translation.Get(TRS.BEGIN_HOUR),
                    EndTimeLabel = Translation.Get(TRS.END_HOUR),
                };

                BindingTrainingDay.BeginTime = _trainingDay.BeginHour.ToLocalTime().TimeOfDay;
				BindingTrainingDay.EndTime = _trainingDay.EndHour.ToLocalTime().TimeOfDay;
            }
            catch (Exception except)
            {
                await _userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
        }

        private async Task ValidateActionAsync()
        {
            try
            {
                if (await SaveDataAsync())
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

        private async Task<bool> SaveDataAsync()
        {
            bool result = false;
            
            _trainingDay.BeginHour = DateTime.Now.Date.Add(BindingTrainingDay.BeginTime).ToUniversalTime();
            _trainingDay.EndHour = DateTime.Now.Date.Add(BindingTrainingDay.EndTime).ToUniversalTime();
            
            if (_editMode == TEditMode.Create)
            {
                _trainingDay.TrainingDayId = 0; // force calculate id
                var trainingDayCreated = await TrainingDayWebService.CreateTrainingDaysAsync(_trainingDay);
                if (trainingDayCreated != null)
                {
                    _trainingDay.TrainingDayId = trainingDayCreated.TrainingDayId;
                    var trainingDayScenario = new TrainingDayScenario() { ManageExercise = true };
                    _trainingDayService.UpdateTrainingDay(trainingDayCreated, trainingDayScenario);
                    result = true;
                }
            }
            else if (_editMode == TEditMode.Edit)
            {
                var trainingDayScenario = new TrainingDayScenario() { ManageExercise = true };
                var trainingDayUpdated = await TrainingDayWebService.UpdateTrainingDayAsync(_trainingDay, trainingDayScenario);
                if (trainingDayUpdated != null)
                {
                    _trainingDayService.UpdateTrainingDay(trainingDayUpdated, trainingDayScenario);
                    result = true;
                }
            }

            return result;
        }

        #region Binding Properties

        private BindingTrainingDay _bindingTrainingDay;
        public BindingTrainingDay BindingTrainingDay
        {
            get { return _bindingTrainingDay; }
            set
            {
                _bindingTrainingDay = value;
                OnPropertyChanged();
            }
        }

        private string _createTitle;
        
        public string CreateTitle
        {
            get { return _createTitle; }
            set
            {
                _createTitle = value;
                OnPropertyChanged();
            }
        }

        private string _yearLabel;

        public string YearLabel
        {
            get { return _yearLabel; }
            set
            {
                _yearLabel = value;
                OnPropertyChanged();
            }
        }

        private string _weekOfYearLabel;

        public string WeekOfYearLabel
        {
            get { return _weekOfYearLabel; }
            set
            {
                _weekOfYearLabel = value;
                OnPropertyChanged();
            }
        }

        private string _dayLabel;

        public string DayLabel
        {
            get { return _dayLabel; }
            set
            {
                _dayLabel = value;
                OnPropertyChanged();
            }
        }

        private string _validateLabel;

        public string ValidateLabel
        {
            get { return _validateLabel; }
            set
            {
                _validateLabel = value;
                OnPropertyChanged();
            }
        }

        #endregion

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
