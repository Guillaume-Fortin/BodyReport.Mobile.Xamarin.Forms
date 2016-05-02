using Acr.UserDialogs;
using BodyReportMobile.Core.Data;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Message.Binding;
using BodyReportMobile.Core.ServiceManagers;
using BodyReportMobile.Core.WebServices;
using Framework;
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
    public class TrainingWeekViewModel : BaseViewModel
    {
        private SQLiteConnection _dbContext;
        private TrainingWeekManager _trainingWeekManager;
        private IUserDialogs _userDialog;

        private TrainingWeekKey _trainingWeekKey;
        public TrainingWeek TrainingWeek { get; set; }

        public TrainingWeekViewModel() : base()
        {
            _dbContext = Resolver.Resolve<ISQLite>().GetConnection();
            _trainingWeekManager = new TrainingWeekManager(_dbContext);
            _userDialog = Resolver.Resolve<IUserDialogs>();
        }

        protected override async void Show()
        {
            base.Show();

            await SynchronizeData();
        }

        protected override void InitTranslation()
        {
            base.InitTranslation();
            TitleLabel = Translation.Get(TRS.TRAINING_WEEK);
            YearLabel = Translation.Get(TRS.YEAR);
            WeekNumberLabel = Translation.Get(TRS.WEEK_NUMBER);
            TrainingDayLabel = Translation.Get(TRS.TRAINING_DAY);
            MondayLabel = Translation.Get(TRS.MONDAY);
            TuesdayLabel = Translation.Get(TRS.TUESDAY);
            WednesdayLabel = Translation.Get(TRS.WEDNESDAY);
            ThursdayLabel = Translation.Get(TRS.THURSDAY);
            FridayLabel = Translation.Get(TRS.FRIDAY);
            SaturdayLabel = Translation.Get(TRS.SATURDAY);
            SundayLabel = Translation.Get(TRS.SUNDAY);

            string weightUnit = "kg", lengthUnit = "cm", unit = Translation.Get(TRS.METRIC);
            var userInfo = UserData.Instance.UserInfo;
            if (userInfo.Unit == (int)TUnitType.Imperial)
            {
                weightUnit = Translation.Get(TRS.POUND);
                lengthUnit = Translation.Get(TRS.INCH);
                unit = Translation.Get(TRS.IMPERIAL);
            }
            HeightLabel = Translation.Get(TRS.HEIGHT) + " (" + lengthUnit + ")";
            WeightLabel = Translation.Get(TRS.WEIGHT) + " (" + weightUnit + ")";

            UserNameLabel = Translation.Get(TRS.USER_NAME) +" : " +"Thetyne";

            OnPropertyChanged(null);
        }

        public static async Task<bool> Show(TrainingWeekKey trainingWeekKey, BaseViewModel parent = null)
        {
            var viewModel = new TrainingWeekViewModel();
            viewModel._trainingWeekKey = trainingWeekKey;
            return await ShowModalViewModel(viewModel, parent);
        }

        private async Task SynchronizeData()
        {
            try
            {
                if (_trainingWeekKey != null)
                {
                    TrainingWeek = await TrainingWeekService.GetTrainingWeek(_trainingWeekKey, true);

                    if (TrainingWeek != null && TrainingWeek.WeekOfYear > 0)
                    {
                        DateTime date = Utils.YearWeekToPlanningDateTime(TrainingWeek.Year, TrainingWeek.WeekOfYear);
                        string dateStr = string.Format(Translation.Get(TRS.FROM_THE_P0TH_TO_THE_P1TH_OF_P2_P3), date.Day, date.AddDays(6).Day, Translation.Get(((TMonthType)date.Month).ToString().ToUpper()), date.Year);

                        TrainingWeek.WeekOfYearDescription = dateStr;
                    }
                    else
                        TrainingWeek.WeekOfYearDescription = string.Empty;

                    OnPropertyChanged("TrainingWeek");
                }
            }
            catch (Exception except)
            {
                await _userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
        }

        public ICommand ViewTrainingDayCommand
        {
            get
            {
                return new Command(async (dayOfWeekParameter) =>
                {
                    if(dayOfWeekParameter != null && dayOfWeekParameter is DayOfWeek)
                        await ViewTrainingDay((DayOfWeek)dayOfWeekParameter);
                });
            }
        }

        private async Task ViewTrainingDay(DayOfWeek dayOfWeek)
        {
            if (BlockUIAction)
                return;

            try
            {
                ActionIsInProgress = true;

                //TODO check training day exist. if not exist, display Create training day

                //TODO view training day
                if(TrainingWeek.TrainingDays != null)
                {
                    var trainingDay = TrainingWeek.TrainingDays.Where(td => td.TrainingDayId == (int)dayOfWeek).FirstOrDefault();
                    if(trainingDay != null)
                    { //view training day
                       
                    }
                }
            }
            catch
            {
            }
            finally
            {
                ActionIsInProgress = false;
            }
        }


        #region Properties binding

        public string UserNameLabel { get; set; }
        public string YearLabel { get; set; }
        public string WeekNumberLabel { get; set; }
        public string WeightLabel { get; set; }
        public string HeightLabel { get; set; }
        public string TrainingDayLabel { get; set; }

        public string MondayLabel { get; set; }
        public string TuesdayLabel { get; set; }
        public string WednesdayLabel { get; set; }
        public string ThursdayLabel { get; set; }
        public string FridayLabel { get; set; }
        public string SaturdayLabel { get; set; }
        public string SundayLabel { get; set; }

        #endregion
    }
}
