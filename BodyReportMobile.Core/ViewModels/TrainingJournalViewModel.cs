using System;
using BodyReportMobile.Core.ViewModels;
using Message;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Threading.Tasks;
using BodyReportMobile.Core.ServiceManagers;
using SQLite.Net;
using BodyReportMobile.Core.Message.Binding;
using Framework;
using XLabs.Ioc;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Framework.Binding;
using BodyReportMobile.Core.WebServices;
using Xamarin.Forms;
using BodyReportMobile.Core.Data;
using System.Globalization;
using Acr.UserDialogs;

namespace BodyReportMobile.Core.ViewModels
{
	public class TrainingJournalViewModel : BaseViewModel
	{
		List<TrainingWeek> _trainingWeekList = null;
		public ObservableCollection<GenericGroupModelCollection<BindingTrainingWeek>> GroupedTrainingWeeks { get; set; } = new ObservableCollection<GenericGroupModelCollection<BindingTrainingWeek>>();

		private SQLiteConnection _dbContext;
		private TrainingWeekManager _trainingWeekManager;
		private bool isBusy;

		private string _createLabel = string.Empty;

		public TrainingJournalViewModel () : base()
        {
			_dbContext = Resolver.Resolve<ISQLite> ().GetConnection ();
			_trainingWeekManager = new TrainingWeekManager (_dbContext);
		}

		protected async override void Show()
		{
			base.Show();

            RetreiveLocalData();
            SynchronizeData();

            await RetreiveAndSaveOnlineData ();
            SynchronizeData();
        }

		protected override void InitTranslation()
		{
			base.InitTranslation ();

			TitleLabel = Translation.Get (TRS.TRAINING_JOURNAL);
			CreateLabel = Translation.Get (TRS.CREATE);
		}

        private void RetreiveLocalData()
        {
            _trainingWeekList = _trainingWeekManager.FindTrainingWeek(null, false);
        }

        private async Task<bool> RetreiveAndSaveOnlineData ()
		{
            bool result = false;
			try
			{
				if (IsBusy)
					return false;
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

					_trainingWeekList = new List<TrainingWeek> ();
					foreach (var trainingWeek in onlineTrainingWeekList)
                        _trainingWeekList.Add (_trainingWeekManager.UpdateTrainingWeek (trainingWeek));
				}
                IsBusy = false;
                result = true;
            }
			catch (Exception except)
			{
                IsBusy = false;
            }
            return result;

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
						Date = string.Format(Translation.Get(TRS.FROM_THE_P0TH_TO_THE_P1TH_OF_P2_P3), dateTime.Day, dateTime.AddDays(6.0d).Day, Translation.Get(((TMonthType)dateTime.Month).ToString().ToUpper()), dateTime.Year),
						Week = Translation.Get(TRS.WEEK_NUMBER) + ' ' + trainingWeek.WeekOfYear.ToString (),
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
				return new Command(async () => { await RetreiveAndSaveOnlineData(); SynchronizeData(); });
			}
		}

		public ICommand CreateNewCommand
		{
			get
			{
				return new Command (async () => { await CreateNewTrainingWeek(); });
			}
		}

		private async Task CreateNewTrainingWeek ()
		{
            var userInfo = UserData.Instance.UserInfo;
            if (userInfo == null)
                userInfo = new UserInfo();

            DateTime dateTime = DateTime.Now;
            var trainingWeek = new TrainingWeek () {
                UserId = userInfo.UserId,
                Year = dateTime.Year,
				WeekOfYear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday),
                UserHeight = userInfo.Height,
				UserWeight = userInfo.Weight,
                Unit = userInfo.Unit
        };

			if (await EditTrainingWeekViewModel.Show (trainingWeek, TEditMode.Create, this))
			{
                //Refresh data
                RetreiveLocalData();
                SynchronizeData();
			}
		}

		public ICommand CopyCommand
		{
			get
			{
				return new Command (async (bindingTrainingWeek) =>
                {
                    try
                    {
                        if (bindingTrainingWeek == null || !(bindingTrainingWeek is BindingTrainingWeek))
                            return;

                        var trainingWeek = (bindingTrainingWeek as BindingTrainingWeek).TrainingWeek;
                        if (trainingWeek != null)
                        {
                            if (await CopyTrainingWeekViewModel.Show(trainingWeek, this))
                            {
                                //Refresh data
                                RetreiveLocalData();
                                SynchronizeData();
                            }
                        }
                    }
                    catch (Exception except)
                    {
                        var userDialog = Resolver.Resolve<IUserDialogs>();
                        await userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
                    }
                });
			}
		}

		public ICommand DeleteCommand
		{
			get
			{
				return new Command (async (bindingTrainingWeek) =>
				{
                    try
                    {
                        if (bindingTrainingWeek == null || !(bindingTrainingWeek is BindingTrainingWeek))
                            return;

                        var trainingWeek = (bindingTrainingWeek as BindingTrainingWeek).TrainingWeek;
                        if (trainingWeek != null)
                        {
                            await TrainingWeekService.DeleteTrainingWeekByKey(trainingWeek as TrainingWeek);
                            bool onlineDataRefreshed = await RetreiveAndSaveOnlineData();
                            if (!onlineDataRefreshed)
                            {
                                // delete data in local database
                                _trainingWeekManager.DeleteTrainingWeek(trainingWeek);
                            }
                            //Refresh data
                            RetreiveLocalData();
                            SynchronizeData();
                        }
                    }
                    catch(Exception except)
                    {
                        var userDialog = Resolver.Resolve<IUserDialogs>();
                        await userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
                    }
				});
			}
		}

		#region accessor

		public string CreateLabel {
			get {
				return _createLabel;
			}
			set {
				_createLabel = value;
				OnPropertyChanged ();
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
                OnPropertyChanged();
            }
		}

		#endregion
	}
}

