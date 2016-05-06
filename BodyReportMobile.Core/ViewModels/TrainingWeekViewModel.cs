using Acr.UserDialogs;
using BodyReportMobile.Core.Data;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Framework.Binding;
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
        
        public TrainingWeek TrainingWeek { get; set; }

        public TrainingWeekViewModel() : base()
        {
            ShowDelayInMs = 0;
            _dbContext = Resolver.Resolve<ISQLite>().GetConnection();
            _trainingWeekManager = new TrainingWeekManager(_dbContext);
            _userDialog = Resolver.Resolve<IUserDialogs>();
            
            for (int i=0; i < BindingWeekTrainingDays.Length; i++)
            {
                BindingWeekTrainingDays[i] = new BindingWeekTrainingDay()
                {
                    DayOfWeek = i == 6 ? DayOfWeek.Sunday : (DayOfWeek)i + 1
                };
            }
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
            
            foreach (var bindingWeekTrainingDay in BindingWeekTrainingDays)
            {
                bindingWeekTrainingDay.Label = Translation.Get(bindingWeekTrainingDay.DayOfWeek.ToString().ToUpper());
            }
            OnPropertyChanged("BindingWeekTrainingDays");

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
            bool result = false;
            if (trainingWeekKey != null)
            {
                var trainingWeek = await TrainingWeekWebService.GetTrainingWeek(trainingWeekKey, true);
                if (trainingWeek != null)
                {
                    var viewModel = new TrainingWeekViewModel();
                    viewModel.TrainingWeek = trainingWeek;
                    result = await ShowModalViewModel(viewModel, parent);
                }
            }

            return await Task.FromResult<bool>(result);
        }

        private void FillWeekOfYearDescription(TrainingWeek trainingWeek)
        {
            if (trainingWeek != null && trainingWeek.WeekOfYear > 0)
            {
                DateTime date = Utils.YearWeekToPlanningDateTime(trainingWeek.Year, trainingWeek.WeekOfYear);
                string dateStr = string.Format(Translation.Get(TRS.FROM_THE_P0TH_TO_THE_P1TH_OF_P2_P3), date.Day, date.AddDays(6).Day, Translation.Get(((TMonthType)date.Month).ToString().ToUpper()), date.Year);

                trainingWeek.WeekOfYearDescription = dateStr;
            }
            else
                trainingWeek.WeekOfYearDescription = string.Empty;
        }

        private void FillWeekDays(TrainingWeek trainingWeek)
        {
            foreach (var bindingWeekTrainingDay in BindingWeekTrainingDays)
                bindingWeekTrainingDay.TrainingDayExist = false;

            if (trainingWeek != null && trainingWeek.TrainingDays != null)
            {
                foreach (var trainingDay in trainingWeek.TrainingDays)
                {
                    foreach (var bindingWeekTrainingDay in BindingWeekTrainingDays)
                    {
                        if(trainingDay.DayOfWeek == (int)bindingWeekTrainingDay.DayOfWeek)
                        {
                            bindingWeekTrainingDay.TrainingDayExist = true;
                        }
                    }
                }
            }
        }

        private async Task SynchronizeData()
        {
            try
            {
                ActionIsInProgress = true;

                FillWeekOfYearDescription(TrainingWeek);
                FillWeekDays(TrainingWeek);
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
                
                if(TrainingWeek.TrainingDays != null)
                {
                    //Check training day exist. if not exist, create new training day
                    var trainingDay = TrainingWeek.TrainingDays.Where(td => td.DayOfWeek == (int)dayOfWeek).FirstOrDefault();
                    if (trainingDay == null)
                    {
                        var newTrainingDay = new TrainingDay()
                        {
                            Year = TrainingWeek.Year,
                            WeekOfYear = TrainingWeek.WeekOfYear,
                            DayOfWeek = (int)dayOfWeek,
                            TrainingDayId = 0,
                            UserId = UserData.Instance.UserInfo.UserId
                        };
                        if(await CreateTrainingDayViewModel.Show(newTrainingDay, this))
                        {
                            TrainingWeek.TrainingDays.Add(newTrainingDay);
                            trainingDay = newTrainingDay;
                            FillWeekDays(TrainingWeek);
                        }
                    }

                    if (trainingDay != null)
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
        public BindingWeekTrainingDay[] BindingWeekTrainingDays { get; set; } = new BindingWeekTrainingDay[7];

        #endregion
    }
}
