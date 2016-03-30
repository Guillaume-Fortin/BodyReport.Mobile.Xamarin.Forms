using System;
using MvvmCross.Plugins.Messenger;
using BodyReportMobile.Core.ViewModels;
using BodyReportMobile.Core.FrameWork.Binding;
using Message;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using BodyReportMobile.Core.ServiceManagers;
using MvvmCross.Platform;
using SQLite.Net;
using Acr.UserDialogs;
using BodyReportMobile.Core.Message.Binding;
using Framework;

namespace BodyReportMobile.Core
{
	public class TrainingJournalViewModel : BaseViewModel
	{
		List<TrainingWeek> _trainingWeekList = null;
		public ObservableCollection<GenericGroupModelCollection<BindingTrainingWeek>> GroupedTrainingWeeks { get; set; } = new ObservableCollection<GenericGroupModelCollection<BindingTrainingWeek>>();

		private SQLiteConnection _dbContext;
		private TrainingWeekManager _trainingWeekManager;
		private bool isBusy;

		private string _createLabel = string.Empty;

		public TrainingJournalViewModel (IMvxMessenger messenger) : base (messenger)
		{
			_dbContext = Mvx.Resolve<ISQLite> ().GetConnection ();
			_trainingWeekManager = new TrainingWeekManager (_dbContext);
		}

		public override void Init (string viewModelGuid, bool autoClearViewModelDataCollection)
		{
			base.Init (viewModelGuid, autoClearViewModelDataCollection);

			_trainingWeekList = _trainingWeekManager.FindTrainingWeek (null, false);
			SynchronizeData ();

			RetreiveAndSaveOnlineData ();
		}

		protected override void InitTranslation()
		{
			base.InitTranslation ();

			TitleLabel = Translation.Get (TRS.TRAINING_JOURNAL);
			CreateLabel = Translation.Get (TRS.CREATE);
		}

		private async Task RetreiveAndSaveOnlineData ()
		{
			try
			{
				if (IsBusy)
					return;
				IsBusy = true;

				var onlineTrainingWeekList = await TrainingWeekService.FindTrainingWeeks ();
				if (onlineTrainingWeekList != null)
				{
					var list = _trainingWeekManager.FindTrainingWeek (null, true);
					if (list != null)
					{
						foreach (var trainingWeek in list)
							_trainingWeekManager.DeleteTrainingWeek (trainingWeek);
					}

					var trainingWeekList = new List<TrainingWeek> ();
					foreach (var trainingWeek in onlineTrainingWeekList)
						trainingWeekList.Add (_trainingWeekManager.UpdateTrainingWeek (trainingWeek));
					SynchronizeData ();
				}
			}
			catch (Exception except)
			{
				// TODO Exception
				//var userDialog = Mvx.Resolve<IUserDialogs> ();
				//userDialog.AlertAsync (except.Message, "Exception", "ok");
			}

			IsBusy = false;
		}

		public void SynchronizeData ()
		{
			//Create BindingCollection
			int currentYear = 0;
			GroupedTrainingWeeks.Clear ();

			if (_trainingWeekList != null)
			{
				_trainingWeekList = _trainingWeekList.OrderByDescending (m => m.Year).ThenByDescending (m => m.WeekOfYear).ToList ();

				DateTime dateTime;
				var localGroupedTrainingWeeks = new ObservableCollection<GenericGroupModelCollection<BindingTrainingWeek>> ();
				GenericGroupModelCollection<BindingTrainingWeek> collection = null;
				foreach (var trainingWeek in _trainingWeekList)
				{
					if (collection == null || currentYear != trainingWeek.Year)
					{
						currentYear = trainingWeek.Year;
						collection = new GenericGroupModelCollection<BindingTrainingWeek> ();
						collection.LongName = currentYear.ToString ();
						collection.ShortName = currentYear.ToString ();
						localGroupedTrainingWeeks.Add (collection);
					}

					dateTime = Utils.YearWeekToPlanningDateTime(trainingWeek.Year, trainingWeek.WeekOfYear);
					collection.Add (new BindingTrainingWeek () {
						//date = string.Format(Translation.FROM_THE_P0TH_TO_THE_P1TH_OF_P2_P3, dateTime.Day, dateTime.AddDays(6).Day, Translation.Get(((TMonthType)dateTime.Month).ToString().ToUpper()), dateTime.Year);
						Date = string.Format(Translation.Get(TRS.FROM_THE_P0TH_TO_THE_P1TH_OF_P2_P3), dateTime.Day, dateTime.AddDays(6.0d).Day, Translation.Get(((TMonthType)dateTime.Month).ToString().ToUpper()), dateTime.Year),
						Week = Translation.Get(TRS.WEEK_NUMBER) + trainingWeek.WeekOfYear.ToString (),
						TrainingWeek = trainingWeek
					});
				}

				foreach (var trainingWeek in localGroupedTrainingWeeks)
				{
					GroupedTrainingWeeks.Add (trainingWeek);
				}
			}
		}

		public ICommand RefreshDataCommand
		{
			get
			{
				return new MvxAsyncCommand (RetreiveAndSaveOnlineData, null, true);
			}
		}

		public ICommand CreateNewCommand
		{
			get
			{
				return new MvxAsyncCommand (CreateNewTrainingWeek, null, true);
			}
		}

		private async Task CreateNewTrainingWeek ()
		{
			var trainingWeek = new TrainingWeek () {
				Year = 2016,
				WeekOfYear = 9,
				UserHeight = 193,
				UserWeight = 90
			};

			if (await EditTrainingWeekViewModel.Show (trainingWeek, TEditMode.Create, this))
			{
				_trainingWeekList.Add (trainingWeek);
				SynchronizeData ();
			}
		}

		public ICommand CopyCommand
		{
			get
			{
				return new MvxAsyncCommand (async () =>
				{
					await ShowModalViewModel<SecondViewModel> (this);
				}, null, true);
			}
		}

		public ICommand DeleteCommand
		{
			get
			{
				return new MvxAsyncCommand (async () =>
				{
					await ShowModalViewModel<ThirdViewModel> (this);
				}, null, true);
			}
		}

		#region accessor

		public string CreateLabel {
			get {
				return _createLabel;
			}
			set {
				_createLabel = value;
				RaisePropertyChanged (() => CreateLabel);
			}
		}

		public bool IsBusy
		{
			get { return isBusy; }
			set
			{
				if (isBusy == value)
					return;

				isBusy = value;
				RaisePropertyChanged (() => IsBusy);
			}
		}

		#endregion
	}
}

