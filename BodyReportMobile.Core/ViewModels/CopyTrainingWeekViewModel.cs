using Acr.UserDialogs;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.ServiceManagers;
using BodyReportMobile.Core.WebServices;
using Message;
using Message.WebApi;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XLabs.Ioc;

namespace BodyReportMobile.Core.ViewModels
{
    public class CopyTrainingWeekViewModel : BaseViewModel
    {
        private SQLiteConnection _dbContext;
        private TrainingWeekManager _trainingWeekManager;

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
            _trainingWeekManager = new TrainingWeekManager(_dbContext);
        }

        protected override void InitTranslation()
        {
            base.InitTranslation();

            TitleLabel = Translation.Get(TRS.TRAINING_WEEK) + ". " + Translation.Get(TRS.COPY);
            ValidateLabel = Translation.Get(TRS.VALIDATE);
            OriginTrainingWeekLabel = "Origin training week";
            NewTrainingWeekLabel = "New training week";
            YearLabel = Translation.Get(TRS.YEAR);
            WeekNumberLabel = Translation.Get(TRS.WEEK_NUMBER);
            OnPropertyChanged(null);
        }

        protected override void Show()
        {
            base.Show();
            SynchronizeData();
        }

        public static async Task<bool> Show(TrainingWeek originTrainingWeek, BaseViewModel parent = null)
        {
            var viewModel = new CopyTrainingWeekViewModel();
            viewModel._originTrainingWeek = originTrainingWeek;
            return await ShowModalViewModel(viewModel, parent);
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

        public ICommand ValidateCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await Validate();
                });
            }
        }

        private async Task Validate()
        {
            try
            {
                if (await ValidateFields() && await SaveData())
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

        private async Task<bool> ValidateFields()
        {
            var userDialog = Resolver.Resolve<IUserDialogs>();
            bool result = false;
            // check NewTrainingWeek not empty and NewTrainingWeek != OriginTrainingWeek
            if (CopyTrainingWeek != null && CopyTrainingWeek.Year > 0 && CopyTrainingWeek.WeekOfYear > 0 &&
               (CopyTrainingWeek.Year != CopyTrainingWeek.OriginYear || CopyTrainingWeek.WeekOfYear != CopyTrainingWeek.OriginWeekOfYear))

            {
                //Check new training doesn't exist
                var key = new TrainingWeekKey() { UserId = CopyTrainingWeek.UserId, Year = CopyTrainingWeek.Year, WeekOfYear = CopyTrainingWeek.WeekOfYear };
                var trainingWeek = await TrainingWeekService.GetTrainingWeek(key);
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

        private async Task<bool> SaveData()
        {
            bool result = false;
            TrainingWeek trainingWeek = await TrainingWeekService.CopyTrainingWeek(CopyTrainingWeek);
            if(trainingWeek != null)
            {
                _trainingWeekManager.DeleteTrainingWeek(trainingWeek);
                _trainingWeekManager.CreateTrainingWeek(trainingWeek);
                result = true;
            }
            return result;
        }
    }
}
