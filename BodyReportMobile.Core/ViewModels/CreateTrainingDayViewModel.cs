using Acr.UserDialogs;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Message.Binding;
using BodyReportMobile.Core.ServiceManagers;
using BodyReportMobile.Core.WebServices;
using Message;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XLabs.Ioc;

namespace BodyReportMobile.Core.ViewModels
{
    public class CreateTrainingDayViewModel : BaseViewModel
    {
        private SQLiteConnection _dbContext;
        private TrainingWeekManager _trainingWeekManager;
        private IUserDialogs _userDialog;

        private TrainingDay _trainingDay;

        public CreateTrainingDayViewModel() : base()
        {
            ShowDelayInMs = 0;
            _dbContext = Resolver.Resolve<ISQLite>().GetConnection();
            _trainingWeekManager = new TrainingWeekManager(_dbContext);
            _userDialog = Resolver.Resolve<IUserDialogs>();
        }

        protected override async void Show()
        {
            base.Show();

            await SynchronizeData();
        }

        public static async Task<bool> Show(TrainingDay trainingDay, BaseViewModel parent = null)
        {
            var viewModel = new CreateTrainingDayViewModel();
            viewModel._trainingDay = trainingDay;
            return await ShowModalViewModel(viewModel, parent);
        }

        protected override void InitTranslation()
        {
            base.InitTranslation();
            TitleLabel = Translation.Get(TRS.TRAINING_DAY);
            CreateTitle = Translation.Get(TRS.CREATE);
            YearLabel = Translation.Get(TRS.YEAR);
            WeekOfYearLabel = Translation.Get(TRS.WEEK_NUMBER);
            DayLabel = Translation.Get(TRS.DAY_OF_WEEK);
            ValidateLabel = Translation.Get(TRS.VALIDATE);
        }

        private async Task SynchronizeData()
        {
            try
            {
                ActionIsInProgress = true;
                
                BindingTrainingDay = new BindingTrainingDay()
                {
                    UserId = _trainingDay.UserId,
                    Year = _trainingDay.Year,
                    WeekOfYear = _trainingDay.WeekOfYear,
                    Day = _trainingDay.DayOfWeek,
                    DayLabel = Translation.Get(((DayOfWeek)_trainingDay.DayOfWeek).ToString().ToUpper()),
                    BeginTimeLabel = Translation.Get(TRS.BEGIN_HOUR),
                    BeginTime = new TimeSpan(12, 0, 0),
                    EndTimeLabel = Translation.Get(TRS.END_HOUR),
                    EndTime = new TimeSpan(12, 0, 0)
                };
            }
            catch (Exception except)
            {
                await _userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
            finally
            {
                ActionIsInProgress = false;
            }
        }

        public ICommand ValidateCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (ActionIsInProgress)
                        return;
                    try
                    {
                        ActionIsInProgress = true;
                        if (await SaveData())
                        {
                            CloseViewModel();
                        }
                    }
                    catch (Exception except)
                    {
                        var userDialog = Resolver.Resolve<IUserDialogs>();
                        await userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
                    }
                    finally
                    {
                        ActionIsInProgress = false;

                    }
                });
            }
        }

        private async Task<bool> SaveData()
        {
            bool result = false;
            
            _trainingDay.BeginHour = DateTime.Now.Date.Add(BindingTrainingDay.BeginTime);
            _trainingDay.EndHour = DateTime.Now.Date.Add(BindingTrainingDay.EndTime);
            _trainingDay.TrainingDayId = 0; // force calculate id
            var trainingDayCreated = await TrainingDayWebService.CreateTrainingDays(_trainingDay);
            if(trainingDayCreated != null)
            {
                _trainingDay.TrainingDayId = trainingDayCreated.TrainingDayId;
                result = true; 
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
    }
}
