using System;
using BodyReportMobile.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using System.Collections.ObjectModel;
using Message;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BodyReportMobile.Core
{
	public class EditTrainingWeekViewModel : BaseViewModel
	{
		private static readonly string TRAINING_WEEK_VALUE = "P1";

		public TrainingWeek TrainingWeek {get; set;}
		//public ObservableCollection<GenericEntryCellBinding> TrainingWeekData {get; set;} = new ObservableCollection<GenericEntryCellBinding>();

		public EditTrainingWeekViewModel (IMvxMessenger messenger) : base(messenger)
		{
			TrainingWeek = new TrainingWeek(){
				Year = 2015
			};
		}

		public override void Init(string viewModelGuid)
		{
			base.Init (viewModelGuid);

			TrainingWeek = ViewModelDataCollection.Get<TrainingWeek>(ViewModelGuid, TRAINING_WEEK_VALUE);

			//Create BindingCollection
			/*
			TrainingWeekData.Clear();
			TrainingWeekData.Add (new GenericEntryCellBinding (_trainingWeek, "Year", Utils.GetPropertyName (() => _trainingWeek.Year)));
			TrainingWeekData.Add (new GenericEntryCellBinding (_trainingWeek, "Week n°", Utils.GetPropertyName (() => _trainingWeek.WeekOfYear)));
			TrainingWeekData.Add (new GenericEntryCellBinding (_trainingWeek, "Height (cm)", Utils.GetPropertyName (() => _trainingWeek.UserHeight)));
			TrainingWeekData.Add (new GenericEntryCellBinding (_trainingWeek, "Weight (cm)", Utils.GetPropertyName (() => _trainingWeek.UserWeight)));
			*/
		}

		public static async Task Show(TrainingWeek trainingWeek, BaseViewModel parent = null)
		{
			string viewModelGuid = Guid.NewGuid ().ToString();
			ViewModelDataCollection.Push (viewModelGuid, TRAINING_WEEK_VALUE, trainingWeek);

			await ShowModalViewModel<EditTrainingWeekViewModel> (viewModelGuid, parent);
		}
	}
}

