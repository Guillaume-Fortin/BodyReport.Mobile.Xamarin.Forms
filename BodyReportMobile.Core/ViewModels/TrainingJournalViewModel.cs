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
using BodyReportMobile.Core.Manager;
using MvvmCross.Platform;
using SQLite.Net;
using Acr.UserDialogs;

namespace BodyReportMobile.Core
{
	public class BindingTrainingWeek
	{
		public string Date {get; set;}
		public string Week {get; set;}
	}

	public class TrainingJournalViewModel : BaseViewModel
	{
		private List<TrainingWeek> _trainingWeekList = new List<TrainingWeek> ();
		public ObservableCollection<GenericGroupModelCollection<BindingTrainingWeek>> GroupedTrainingWeeks {get; set;} = new ObservableCollection<GenericGroupModelCollection<BindingTrainingWeek>>();
		private SQLiteConnection _dbContext;
		private TrainingWeekManager _trainingWeekManager;

		private bool isBusy;
		public bool IsBusy
		{
			get { return isBusy; }
			set 
			{
				if (isBusy == value)
					return;

				isBusy = value;
				RaisePropertyChanged ("IsBusy");
			}
		}

		public TrainingJournalViewModel (IMvxMessenger messenger) : base(messenger)
		{
			_dbContext = Mvx.Resolve<ISQLite> ().GetConnection ();
			_trainingWeekManager = new TrainingWeekManager(_dbContext);
		}

		public override void Init(string viewModelGuid, bool autoClearViewModelDataCollection)
		{
			base.Init (viewModelGuid, autoClearViewModelDataCollection);

			//Fake get Web Data
			/*for (int i = 2016; i >= 2010; i--) {
				_trainingWeekList.Add (new TrainingWeek (){ Year = i, WeekOfYear = 8 });	
				_trainingWeekList.Add (new TrainingWeek (){ Year = i, WeekOfYear = 7 });	
				_trainingWeekList.Add (new TrainingWeek (){ Year = i, WeekOfYear = 5 });	
				_trainingWeekList.Add (new TrainingWeek (){ Year = i, WeekOfYear = 4 });
			}*/

			_trainingWeekList = _trainingWeekManager.FindTrainingWeek (null, false);
			SynchronizeData ();

			RetreiveAndSaveOnlineData ();
		}

		private async Task RetreiveAndSaveOnlineData()
		{
			try
			{
				if (IsBusy)
					return;
				IsBusy = true;

				var onlineTrainingWeekList = await TrainingWeekService.FindTrainingWeeks ();
				if(onlineTrainingWeekList != null)
				{
					var list = _trainingWeekManager.FindTrainingWeek(null, true);
					if(list != null)
					{
						foreach(var trainingWeek in list)
							_trainingWeekManager.DeleteTrainingWeek(trainingWeek);
					}

					_trainingWeekList = new List<TrainingWeek>();
					foreach(var trainingWeek in onlineTrainingWeekList)
						_trainingWeekList.Add(_trainingWeekManager.UpdateTrainingWeek(trainingWeek));
				}
			}
			catch (Exception except){
				var userDialog = Mvx.Resolve<IUserDialogs> ();
				userDialog.AlertAsync (except.Message, "Exception", "ok");
			}
			SynchronizeData ();
			IsBusy = false;
		}

		public void SynchronizeData()
		{
			//Create BindingCollection
			int currentYear = 0;
			GroupedTrainingWeeks.Clear();

			if (_trainingWeekList != null) {
				var localGroupedTrainingWeeks = new ObservableCollection<GenericGroupModelCollection<BindingTrainingWeek>> ();
				GenericGroupModelCollection<BindingTrainingWeek> collection = null;
				foreach (var trainingWeek in _trainingWeekList) {
					if (collection == null || currentYear != trainingWeek.Year) {
						currentYear = trainingWeek.Year;
						collection = new GenericGroupModelCollection<BindingTrainingWeek> ();
						collection.LongName = currentYear.ToString ();
						collection.ShortName = currentYear.ToString ();
						localGroupedTrainingWeeks.Add (collection);
					}

					collection.Add (new BindingTrainingWeek () {
						Date = "From the 22th to the 28th of February 2016",
						Week = "Week n°" + trainingWeek.WeekOfYear.ToString ()
					});
				}

				foreach (var trainingWeek in localGroupedTrainingWeeks) {
					GroupedTrainingWeeks.Add (trainingWeek);
				}
			}
		}

		public ICommand RefreshDataCommand {
			get {
				return new MvxAsyncCommand (RetreiveAndSaveOnlineData, null, true);
			}
		}

		public ICommand CreateNewCommand {
			get {
				return new MvxAsyncCommand (CreateNewTrainingWeek, null, true);
			}
		}

		private async Task CreateNewTrainingWeek()
		{
			var trainingWeek = new TrainingWeek(){
				Year = 2016,
				WeekOfYear = 9,
				UserHeight = 193,
				UserWeight = 90
			};

			if(await EditTrainingWeekViewModel.Show(trainingWeek, TEditMode.Create, this))
			{
				_trainingWeekList.Add(trainingWeek);
				_trainingWeekList = _trainingWeekList.OrderByDescending(m => m.Year).ThenByDescending(m=>m.WeekOfYear).ToList();
				SynchronizeData();
			}
		}

		public ICommand CopyCommand
		{
			get
			{
				return new MvxAsyncCommand (async () => {
					await ShowModalViewModel<SecondViewModel>(this);
				}, null, true);
			}
		}

		public ICommand DeleteCommand
		{
			get
			{
				return new MvxAsyncCommand (async () => {
					await ShowModalViewModel<ThirdViewModel>(this);
				}, null, true);
			}
		}
	}
}

