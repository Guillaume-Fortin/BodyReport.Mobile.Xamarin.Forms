using System;
using Message;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using System.Collections.Generic;
using Framework;
using Acr.UserDialogs;
using XLabs.Ioc;
using BodyReportMobile.Core.ViewModels.Generic;
using BodyReportMobile.Core.WebServices;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Data;

namespace BodyReportMobile.Core.ViewModels
{
	public class EditTrainingWeekViewModel : BaseViewModel
	{
		private static readonly string TRAINING_WEEK_VALUE = "P1";
		private static readonly string EDIT_MODE = "P2";

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
		}

		#endregion

		public EditTrainingWeekViewModel () : base ()
		{
			TrainingWeek = new TrainingWeek () {
				Year = 2015
			};
		}

		public override void Init (string viewModelGuid, bool autoClearViewModelDataCollection)
		{
			TrainingWeek = ViewModelDataCollection.Get<TrainingWeek> (viewModelGuid, TRAINING_WEEK_VALUE);
			EditMode = ViewModelDataCollection.Get<TEditMode> (viewModelGuid, EDIT_MODE);

			base.Init (viewModelGuid, autoClearViewModelDataCollection);

			SynchronizeData ();
		}

		public static async Task<bool> Show (TrainingWeek trainingWeek, TEditMode editMode, BaseViewModel parent = null)
		{
			/*string viewModelGuid = Guid.NewGuid ().ToString ();
			ViewModelDataCollection.Push (viewModelGuid, TRAINING_WEEK_VALUE, trainingWeek);
			ViewModelDataCollection.Push (viewModelGuid, EDIT_MODE, editMode);
            */
            var viewModel = new EditTrainingWeekViewModel();
            viewModel.ViewModelGuid = Guid.NewGuid().ToString();
            return await ShowModalViewModel (viewModel, true, parent);
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
				return new MvxAsyncCommand (async () =>
				{
					try
					{
						if (ValidateFields () && await SaveData ())
						{
							CloseViewModel ();
						}
					}
					catch (Exception except)
					{
						var userDialog = Resolver.Resolve<IUserDialogs> ();
						await userDialog.AlertAsync (except.Message, Translation.Get (TRS.ERROR), Translation.Get (TRS.OK));
					}
				}, null, true);
			}
		}

		public ICommand ChangeYearCommand
		{
			get
			{
				return new MvxAsyncCommand (async () =>
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

					if (result.ViewModelValidated && result.SelectedTag != null)
					{
						if (((int)result.SelectedTag) > 0)
							TrainingWeek.Year = (int)result.SelectedTag;
						SynchronizeData ();
					}
				}, null, true);
			}
		}

		public ICommand ChangeWeekOfYearCommand
		{
			get
			{
				return new MvxAsyncCommand (async () =>
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

					if (result.ViewModelValidated && result.SelectedTag != null)
					{
						if (((int)result.SelectedTag) > 0)
							TrainingWeek.WeekOfYear = (int)result.SelectedTag;
						SynchronizeData ();
					}
				}, null, true);
			}
		}

		private bool ValidateFields ()
		{
			return TrainingWeek != null && TrainingWeek.Year > 0 && TrainingWeek.WeekOfYear > 0 &&
			TrainingWeek.UserHeight > 0 && TrainingWeek.UserWeight > 0;
			//TODO verify training week doesn't exist
		}

		private async Task<bool> SaveData ()
		{
			var data = await TrainingWeekService.UpdateTrainingWeek (TrainingWeek);
			return data != null;
		}
	}
}

