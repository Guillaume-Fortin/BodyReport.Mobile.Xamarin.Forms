using System;
using MvvmCross.Plugins.Messenger;
using BodyReportMobile.Core.ViewModels;
using BodyReportMobile.Core.FrameWork.Binding;
using Message;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;

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

		public TrainingJournalViewModel (IMvxMessenger messenger) : base(messenger)
		{
		}

		public override void Init(string viewModelGuid)
		{
			base.Init (viewModelGuid);

			//Fake get Web Data
			for (int i = 2016; i >= 2010; i--) {
				_trainingWeekList.Add (new TrainingWeek (){ Year = i, WeekOfYear = 8 });	
				_trainingWeekList.Add (new TrainingWeek (){ Year = i, WeekOfYear = 7 });	
				_trainingWeekList.Add (new TrainingWeek (){ Year = i, WeekOfYear = 5 });	
				_trainingWeekList.Add (new TrainingWeek (){ Year = i, WeekOfYear = 4 });
			}

			//Create BindingCollection
			int currentYear = 0;
			GroupedTrainingWeeks.Clear();
			GenericGroupModelCollection<BindingTrainingWeek> collection = null;
			foreach (var trainingWeek in _trainingWeekList) {
				if (collection == null || currentYear != trainingWeek.Year) {
					currentYear = trainingWeek.Year;
					collection = new GenericGroupModelCollection<BindingTrainingWeek> ();
					collection.LongName = currentYear.ToString();
					collection.ShortName = currentYear.ToString();
					GroupedTrainingWeeks.Add (collection);
				}

				collection.Add (new BindingTrainingWeek (){ Date = "From the 22th to the 28th of February 2016", Week = "Week n°" + trainingWeek.WeekOfYear.ToString() });
			}
		}

		public ICommand CreateNewCommand
		{
			get
			{
				return new MvxAsyncCommand (async () => {
					var trainingWeek = new TrainingWeek(){
						Year = 2016
					};
					await EditTrainingWeekViewModel.Show(trainingWeek, this);
				});
			}
		}

		public ICommand CopyCommand
		{
			get
			{
				return new MvxAsyncCommand (async () => {
					await ShowModalViewModel<SecondViewModel>(this);
				});
			}
		}

		public ICommand DeleteCommand
		{
			get
			{
				return new MvxAsyncCommand (async () => {
					await ShowModalViewModel<ThirdViewModel>(this);
				});
			}
		}
	}
}

