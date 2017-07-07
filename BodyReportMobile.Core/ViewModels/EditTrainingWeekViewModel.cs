using System;
using BodyReport.Message;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Generic;
using BodyReport.Framework;
using Acr.UserDialogs;
using XLabs.Ioc;
using BodyReportMobile.Core.ViewModels.Generic;
using BodyReportMobile.Core.WebServices;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Data;
using BodyReportMobile.Core.ServiceLayers;

namespace BodyReportMobile.Core.ViewModels
{
	public class EditTrainingWeekViewModel : BaseViewModel
	{
        private ApplicationDbContext _dbContext;
        private TrainingWeekService _trainingWeekService;

        public TEditMode EditMode { get; set; }

		public TrainingWeek TrainingWeek { get; set; }

		#region translation

		public string EditTitle { get; set; }

		public string ValidateLabel { get; set; }

		public string YearLabel { get; set; }

		public string WeekNumberLabel { get; set; }

		public string HeightLabel { get; set; }

		public string WeightLabel { get; set; }

		protected override void InitTranslation ()
		{
			base.InitTranslation ();

			string weightUnit = "kg", lengthUnit = "cm";

			var userInfo = UserData.Instance.UserInfo;
			if (userInfo.Unit == (int)TUnitType.Imperial)
			{
				weightUnit = Translation.Get (TRS.POUND);
				lengthUnit = Translation.Get (TRS.INCH);
			}

			TitleLabel = Translation.Get (TRS.TRAINING_WEEK);
			EditTitle = EditMode == TEditMode.Create ? Translation.Get (TRS.CREATE) : Translation.Get (TRS.EDIT);
			ValidateLabel = EditMode == TEditMode.Create ? Translation.Get (TRS.CREATE) : Translation.Get (TRS.VALIDATE);
			YearLabel = Translation.Get (TRS.YEAR);
			WeekNumberLabel = Translation.Get (TRS.WEEK_NUMBER);
			HeightLabel = Translation.Get (TRS.HEIGHT) + " (" + lengthUnit + ")";
			WeightLabel = Translation.Get (TRS.WEIGHT) + " (" + weightUnit + ")";
            OnPropertyChanged(null);
		}

		#endregion

		public EditTrainingWeekViewModel () : base ()
		{
            _dbContext = Resolver.Resolve<ISQLite>().GetConnection();
            _trainingWeekService = new TrainingWeekService(DbContext);
        }

		protected override async Task ShowAsync ()
		{
            await base.ShowAsync();

			SynchronizeData ();
		}

		public static async Task<bool> ShowAsync (TrainingWeek trainingWeek, TEditMode editMode, BaseViewModel parent = null)
		{
            var viewModel = new EditTrainingWeekViewModel();
            viewModel.TrainingWeek = trainingWeek;
            viewModel.EditMode = editMode;
            return await ShowModalViewModelAsync (viewModel, parent);
		}

		private void SynchronizeData ()
		{
			if (TrainingWeek != null && TrainingWeek.WeekOfYear > 0)
			{
				DateTime date = Utils.YearWeekToPlanningDateTime (TrainingWeek.Year, TrainingWeek.WeekOfYear);
				string dateStr = string.Format (Translation.Get (TRS.FROM_THE_P0TH_TO_THE_P1TH_OF_P2_P3), date.Day, date.AddDays (6).Day, Translation.Get (((TMonthType)date.Month).ToString ().ToUpper ()), date.Year);

				TrainingWeek.WeekOfYearDescription = dateStr;
			}
			else
				TrainingWeek.WeekOfYearDescription = string.Empty;
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

        private async Task ChangeYearActionAsync()
        {
            try
            {
                var datas = new List<Message.GenericData>();

                int currentYear = DateTime.Now.Year;
                Message.GenericData data, currentData = null;
                for (int i = currentYear; i >= currentYear - 1; i--)
                {
                    data = new Message.GenericData() { Tag = i, Name = i.ToString() };
                    datas.Add(data);

                    if (i == TrainingWeek.Year)
                        currentData = data;
                }

                var result = await ListViewModel.ShowGenericListAsync(Translation.Get(TRS.YEAR), datas, currentData, this);

                if (result.Validated && result.SelectedData != null)
                {
                    if (((int)result.SelectedData.Tag) > 0)
                        TrainingWeek.Year = (int)result.SelectedData.Tag;
                    SynchronizeData();
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to change year", except);
            }
        }

        private async Task ChangeWeekOfYearActionAsync()
        {
            try
            {
                var datas = new List<Message.GenericData>();

                String dateStr, labelStr;
                DateTime date;
                Message.GenericData data, currentData = null;
                for (int i = 1; i <= 52; i++)
                {
                    date = Utils.YearWeekToPlanningDateTime(TrainingWeek.Year, i);
                    dateStr = string.Format(Translation.Get(TRS.FROM_THE_P0TH_TO_THE_P1TH_OF_P2_P3), date.Day, date.AddDays(6).Day, Translation.Get(((TMonthType)date.Month).ToString().ToUpper()), date.Year);
                    labelStr = Translation.Get(TRS.WEEK_NUMBER) + ' ' + i;

                    data = new Message.GenericData() { Tag = i, Name = labelStr, Description = dateStr };
                    datas.Add(data);

                    if (i == TrainingWeek.WeekOfYear)
                        currentData = data;
                }

                var result = await ListViewModel.ShowGenericListAsync(Translation.Get(TRS.WEEK_NUMBER), datas, currentData, this);

                if (result.Validated && result.SelectedData != null)
                {
                    if (((int)result.SelectedData.Tag) > 0)
                        TrainingWeek.WeekOfYear = (int)result.SelectedData.Tag;
                    SynchronizeData();
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to change week of year", except);
            }
        }
        
		private async Task<bool> ValidateFieldsAsync()
		{
			if(TrainingWeek != null && TrainingWeek.Year > 0 && TrainingWeek.WeekOfYear > 0 &&
			   TrainingWeek.UserHeight > 0 && TrainingWeek.UserWeight > 0 && !string.IsNullOrWhiteSpace(TrainingWeek.UserId))
            {
                var onlineTrainingWeek = await TrainingWeekWebService.GetTrainingWeekAsync(TrainingWeek);
                if (EditMode == TEditMode.Create)
                {
                    //verify training week doesn't exist
                    if (onlineTrainingWeek != null)
                        throw new Exception(string.Format(Translation.Get(TRS.P0_ALREADY_EXIST), Translation.Get(TRS.TRAINING_WEEK)));
                    return true;
                }
                else
                {
                    //verify training week exist
                    if (onlineTrainingWeek == null)
                        throw new Exception(string.Format(Translation.Get(TRS.P0_NOT_EXIST), Translation.Get(TRS.TRAINING_WEEK)));
                    return true;
                }
            }
            return false;
		}

		private async Task<bool> SaveDataAsync()
		{
            TrainingWeek trainingWeek = null;
            if (EditMode == TEditMode.Create)
            {
                trainingWeek = await TrainingWeekWebService.CreateTrainingWeekAsync(TrainingWeek);
                if (trainingWeek != null)
                {
                    var trainingWeekScenario = new TrainingWeekScenario() { ManageTrainingDay = true };
                    _trainingWeekService.UpdateTrainingWeek(trainingWeek, trainingWeekScenario);
                }
            }
            else
            {
                var trainingWeekScenario = new TrainingWeekScenario() { ManageTrainingDay = false };
                trainingWeek = await TrainingWeekWebService.UpdateTrainingWeekAsync(TrainingWeek, trainingWeekScenario);
                if (trainingWeek != null)
                    _trainingWeekService.UpdateTrainingWeek(trainingWeek, trainingWeekScenario);
            }

            return trainingWeek != null;
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

        private ICommand _changeYearCommand = null;
        public ICommand ChangeYearCommand
        {
            get
            {
                if (_changeYearCommand == null)
                {
                    _changeYearCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await ChangeYearActionAsync();
                    });
                }
                return _changeYearCommand;
            }
        }

        private ICommand _changeWeekOfYearCommand = null;
        public ICommand ChangeWeekOfYearCommand
        {
            get
            {
                if (_changeWeekOfYearCommand == null)
                {
                    _changeWeekOfYearCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await ChangeWeekOfYearActionAsync();
                    });
                }
                return _changeWeekOfYearCommand;
            }
        }

        public ApplicationDbContext DbContext { get => _dbContext; set => _dbContext = value; }

        #endregion
    }
}

