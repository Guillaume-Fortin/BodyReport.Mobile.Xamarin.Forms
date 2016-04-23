using System;
using Message;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Generic;
using Framework;
using Acr.UserDialogs;
using XLabs.Ioc;
using BodyReportMobile.Core.ViewModels.Generic;
using BodyReportMobile.Core.WebServices;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Data;
using Xamarin.Forms;
using SQLite.Net;
using BodyReportMobile.Core.ServiceManagers;

namespace BodyReportMobile.Core.ViewModels
{
	public class EditTrainingWeekViewModel : BaseViewModel
	{
        private SQLiteConnection _dbContext;
        private TrainingWeekManager _trainingWeekManager;

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

			string weightUnit = "kg", lengthUnit = "cm", unit = Translation.Get (TRS.METRIC);

			var userInfo = UserData.Instance.UserInfo;
			if (userInfo.Unit == (int)TUnitType.Imperial)
			{
				weightUnit = Translation.Get (TRS.POUND);
				lengthUnit = Translation.Get (TRS.INCH);
				unit = Translation.Get (TRS.IMPERIAL);
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
            _trainingWeekManager = new TrainingWeekManager(_dbContext);
        }

		protected override void Show ()
		{
            base.Show();


			SynchronizeData ();
		}

		public static async Task<bool> Show (TrainingWeek trainingWeek, TEditMode editMode, BaseViewModel parent = null)
		{
            var viewModel = new EditTrainingWeekViewModel();
            viewModel.TrainingWeek = trainingWeek;
            viewModel.EditMode = editMode;
            return await ShowModalViewModel (viewModel, parent);
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

		public ICommand ValidateCommand
		{
			get
			{
				return new Command (async () =>
				{
                    if (ActionIsInProgress)
                        return;
					try
					{
                        ActionIsInProgress = true;
                        if (await ValidateFields () && await SaveData ())
						{
							CloseViewModel ();
						}
					}
					catch (Exception except)
					{
						var userDialog = Resolver.Resolve<IUserDialogs> ();
						await userDialog.AlertAsync (except.Message, Translation.Get (TRS.ERROR), Translation.Get (TRS.OK));
					}
                    finally
                    {
                        ActionIsInProgress = false;

                    }
				});
			}
		}

		public ICommand ChangeYearCommand
		{
			get
			{
				return new Command(async () =>
				{

					var datas = new List<Message.GenericData> ();

					int currentYear = DateTime.Now.Year;
					Message.GenericData data, currentData = null;
					for (int i = currentYear; i >= currentYear - 1; i--)
					{
						data = new Message.GenericData (){ Tag = i, Name = i.ToString () };
						datas.Add (data);

						if (i == TrainingWeek.Year)
							currentData = data;
					}

					var result = await ListViewModel.ShowGenericList (Translation.Get (TRS.YEAR), datas, currentData, this);

					if (result.Validated && result.SelectedData != null)
					{
						if (((int)result.SelectedData.Tag) > 0)
							TrainingWeek.Year = (int)result.SelectedData.Tag;
						SynchronizeData ();
					}
				});
			}
		}

		public ICommand ChangeWeekOfYearCommand
		{
			get
			{
				return new Command(async () =>
				{

					var datas = new List<Message.GenericData> ();

					String dateStr, labelStr;
					DateTime date;
					Message.GenericData data, currentData = null;
					for (int i = 1; i <= 52; i++)
					{
						date = Utils.YearWeekToPlanningDateTime (TrainingWeek.Year, i);
						dateStr = string.Format (Translation.Get (TRS.FROM_THE_P0TH_TO_THE_P1TH_OF_P2_P3), date.Day, date.AddDays (6).Day, Translation.Get (((TMonthType)date.Month).ToString ().ToUpper ()), date.Year);
						labelStr = Translation.Get (TRS.WEEK_NUMBER) + ' ' + i;

						data = new Message.GenericData (){ Tag = i, Name = labelStr, Description = dateStr };
						datas.Add (data);

						if (i == TrainingWeek.WeekOfYear)
							currentData = data;
					}

					var result = await ListViewModel.ShowGenericList (Translation.Get (TRS.WEEK_NUMBER), datas, currentData, this);

					if (result.Validated && result.SelectedData != null)
					{
						if (((int)result.SelectedData.Tag) > 0)
							TrainingWeek.WeekOfYear = (int)result.SelectedData.Tag;
						SynchronizeData ();
					}
				});
			}
		}

		private async Task<bool> ValidateFields()
		{
			if(TrainingWeek != null && TrainingWeek.Year > 0 && TrainingWeek.WeekOfYear > 0 &&
			TrainingWeek.UserHeight > 0 && TrainingWeek.UserWeight > 0 && !string.IsNullOrWhiteSpace(TrainingWeek.UserId))
            {
                var onlineTrainingWeek = await TrainingWeekService.GetTrainingWeek(TrainingWeek);
                if (EditMode == TEditMode.Create)
                {
                    //verify training week doesn't exist
                    if (onlineTrainingWeek != null)
                        throw new Exception(string.Format(Translation.Get(TRS.P0_ALREADY_EXIST), Translation.Get(TRS.TRAINING_WEEK)));
                    return true;
                }
                else
                {
                    if (onlineTrainingWeek != null)
                    {
                        //verify training week exist
                        if (onlineTrainingWeek == null)
                            throw new Exception(string.Format(Translation.Get(TRS.P0_NOT_EXIST), Translation.Get(TRS.TRAINING_WEEK)));
                        return true; 
                    }
                    return true;
                }
            }
            return false;
		}

		private async Task<bool> SaveData ()
		{
            TrainingWeek trainingWeek = null;
            if (EditMode == TEditMode.Create)
            {
                trainingWeek = await TrainingWeekService.CreateTrainingWeek(TrainingWeek);
                if (trainingWeek != null)
                {
                    _trainingWeekManager.DeleteTrainingWeek(trainingWeek);
                    _trainingWeekManager.CreateTrainingWeek(trainingWeek);
                }
            }
            else
            {
                trainingWeek = await TrainingWeekService.UpdateTrainingWeek(TrainingWeek);
                if (trainingWeek != null)
                {
                    _trainingWeekManager.UpdateTrainingWeek(trainingWeek);
                }
            }

            return trainingWeek != null;
        }
	}
}

