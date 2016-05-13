using Acr.UserDialogs;
using BodyReportMobile.Core.Data;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Framework.Binding;
using BodyReportMobile.Core.Message.Binding;
using BodyReportMobile.Core.ServiceManagers;
using BodyReportMobile.Core.WebServices;
using Message;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XLabs.Ioc;

namespace BodyReportMobile.Core.ViewModels
{
    public class TrainingDayViewModelResut
    {
        public bool Result = false;
        public List<TrainingDay> TrainingDays { get; set; } = new List<TrainingDay>();
    }

    public class TrainingDayViewModel : BaseViewModel
    {
        private SQLiteConnection _dbContext;
        private BodyExerciseManager _bodyExerciseManager;
        private IUserDialogs _userDialog;

        List<BodyExercise> _bodyExerciseList;
        /// <summary>
        /// UserId
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Year
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// Week of year
        /// </summary>
        public int WeekOfYear { get; set; }
        /// <summary>
        /// Day of week
        /// </summary>
        public int DayOfWeek { get; set; }

        private List<TrainingDay> _trainingDays { get; set; } = new List<TrainingDay>();
        public ObservableCollection<GenericGroupModelCollection<BindingTrainingExercise>> GroupedTrainingExercises { get; set; } = new ObservableCollection<GenericGroupModelCollection<BindingTrainingExercise>>();

        private object _locker = new object();
        private CancellationTokenSource _cachingImageCancellationTokenSource = null;

        public TrainingDayViewModel() : base()
        {
            ShowDelayInMs = 0;
            _dbContext = Resolver.Resolve<ISQLite>().GetConnection();
            _bodyExerciseManager = new BodyExerciseManager(_dbContext);
            _userDialog = Resolver.Resolve<IUserDialogs>();
        }

        protected override void Closed()
        {
            base.Closed();
            try
            {
                lock (_locker)
                {
                    if (_cachingImageCancellationTokenSource != null)
                    {
                        _cachingImageCancellationTokenSource.Cancel();
                        _cachingImageCancellationTokenSource = null;
                    }
                }
            }
            catch
            { }
        }

        protected override async void Show()
        {
            base.Show();

            if (_trainingDays != null && _trainingDays.Count > 0)
            {
                var trainingDay = _trainingDays[0];
                UserId = trainingDay.UserId;
                Year = trainingDay.Year;
                WeekOfYear = trainingDay.WeekOfYear;
                DayOfWeek = trainingDay.DayOfWeek;
            }
            await SynchronizeData();
        }

        public static async Task<TrainingDayViewModelResut> Show(List<TrainingDay> trainingDayList, BaseViewModel parent = null)
        {
            TrainingDayViewModelResut trainingDayViewModelResut = new TrainingDayViewModelResut();
            if (trainingDayList != null && trainingDayList.Count > 0)
            {
                var viewModel = new TrainingDayViewModel();
                viewModel._trainingDays = trainingDayList;
                await ShowModalViewModel(viewModel, parent);

                //Here always return true because it's an interactive page, user doesn't validate page
                trainingDayViewModelResut.Result = true;
                trainingDayViewModelResut.TrainingDays.AddRange(viewModel._trainingDays);
            }

            return trainingDayViewModelResut;
        }

        protected override void InitTranslation()
        {
            base.InitTranslation();
            TitleLabel = Translation.Get(TRS.TRAINING_DAY);
            CreateTrainingLabel = Translation.Get(TRS.CREATE_NEW_PE);
            TrainingModeLabel = "Training Mode";
            AddExerciseLabel = Translation.Get(TRS.ADD_EXERCISES);
        }

        private void PopulateBindingTrainingDay(TrainingDay trainingDay)
        {
            if (trainingDay == null)
                return;
            
            string weightUnit = "kg", lengthUnit = "cm", unit = Translation.Get(TRS.METRIC);

            if (UserData.Instance.UserInfo.Unit == (int)TUnitType.Imperial)
            {
                weightUnit = Translation.Get(TRS.POUND);
                lengthUnit = Translation.Get(TRS.INCH);
                unit = Translation.Get(TRS.IMPERIAL);
            }

            StringBuilder setRepSb = new StringBuilder();
            StringBuilder setRepWeightSb = new StringBuilder();
            string formatSetRep = "{0} x {1}";
            string beginHourStr, endHourStr;
            BindingTrainingExercise bindingTrainingExercise;
            var localGroupedTrainingExercises = new ObservableCollection<GenericGroupModelCollection<BindingTrainingExercise>>();
            GenericGroupModelCollection<BindingTrainingExercise> collection = null;

            collection = localGroupedTrainingExercises.Where(lgte => lgte.ReferenceObject == trainingDay).FirstOrDefault();
            if (collection == null)
            {
                collection = new GenericGroupModelCollection<BindingTrainingExercise>();
                localGroupedTrainingExercises.Add(collection);
            }

            beginHourStr = trainingDay.BeginHour == null ? "00:00" : trainingDay.BeginHour.ToString("HH:mm");
            endHourStr = trainingDay.EndHour == null ? "00:00" : trainingDay.EndHour.ToString("HH:mm");
            collection.LongName = string.Format("{0} {1} {2} {3}", Translation.Get(TRS.FROM), beginHourStr, Translation.Get(TRS.TO), endHourStr);
            collection.ShortName = collection.LongName;
            collection.ReferenceObject = trainingDay;
            collection.Clear();

            if (trainingDay.TrainingExercises != null)
            {
                foreach (var trainingExercise in trainingDay.TrainingExercises)
                {
                    var bodyExercise = _bodyExerciseList.Where(m => m.Id == trainingExercise.BodyExerciseId).FirstOrDefault();
                    bindingTrainingExercise = new BindingTrainingExercise()
                    {
                        BodyExerciseId = trainingExercise.Id,
                        RestTime = trainingExercise.RestTime,
                        Image = HttpConnector.Instance.BaseUrl + "images/bodyexercises/" + bodyExercise.ImageName
                    };
                    bindingTrainingExercise.BodyExerciseName = bodyExercise.Name;
                    if (trainingExercise.TrainingExerciseSets != null)
                    {
                        bindingTrainingExercise.SetRepsTitle = string.Format(formatSetRep, Translation.Get(TRS.SETS), Translation.Get(TRS.REPS));
                        bindingTrainingExercise.SetRepWeightsTitle = Translation.Get(TRS.WEIGHT) + " (" + weightUnit + ")";
                        setRepSb.Clear();
                        setRepWeightSb.Clear();
                        foreach (var trainingExerciseSet in trainingExercise.TrainingExerciseSets)
                        {
                            setRepSb.AppendLine(string.Format(formatSetRep, trainingExerciseSet.NumberOfSets, trainingExerciseSet.NumberOfReps));
                            setRepWeightSb.AppendLine(trainingExerciseSet.Weight.ToString());
                        }
                        bindingTrainingExercise.SetReps = setRepSb.ToString();
                        bindingTrainingExercise.SetRepWeights = setRepWeightSb.ToString();
                    }
                    collection.Add(bindingTrainingExercise);
                }

                if (collection != null && collection.Count > 0)
                {
                    Task t = CachingImages(collection);
                }
            }
            
            foreach (var trainingExercise in localGroupedTrainingExercises)
            {
                GroupedTrainingExercises.Add(trainingExercise);
            }
        }

        List<BindingTrainingExercise> _cachingBindingList = new List<BindingTrainingExercise>();

        public async Task CachingImages(GenericGroupModelCollection<BindingTrainingExercise> bindingGenericTrainingExercise)
        {
            lock (_locker)
            {
                if (_cachingImageCancellationTokenSource != null)
                {
                    _cachingImageCancellationTokenSource.Cancel();
                    _cachingImageCancellationTokenSource = null;
                }
                _cachingImageCancellationTokenSource = new CancellationTokenSource();
            }

            lock (_cachingBindingList)
            {
                _cachingBindingList.Clear();
                _cachingBindingList.AddRange(bindingGenericTrainingExercise);
            }

            string imageName, urlImage, localImagePath;
            string urlImages = HttpConnector.Instance.BaseUrl + "images/bodyexercises/{0}";
            List<Task> taskList = null;
            foreach (var bindingTrainingExercise in bindingGenericTrainingExercise)
            {
                if (_cachingImageCancellationTokenSource.Token.IsCancellationRequested)
                    _cachingImageCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (taskList == null)
                    taskList = new List<Task>();

                if (bindingTrainingExercise != null)
                {
                    imageName = bindingTrainingExercise.BodyExerciseId.ToString() + ".png";
                    urlImage = string.Format(urlImages, imageName);
                    localImagePath = Path.Combine(AppTools.BodyExercisesImagesDirectory, imageName);
                    var t = AppTools.Instance.CachingImage(bindingTrainingExercise.GetHashCode(), urlImage, localImagePath, (cachingImageResult) =>
                    {
                        if (cachingImageResult != null)
                        {
                            lock (_cachingBindingList)
                            {
                                var bindingBodyExerciseTmp = _cachingBindingList.Where(be => be.GetHashCode() == cachingImageResult.IdImage).FirstOrDefault();
                                if (bindingBodyExerciseTmp != null)
                                    bindingBodyExerciseTmp.Image = cachingImageResult.ImagePath;
                            }
                        }
                    });
                    taskList.Add(t);
                }
            }

            if (taskList != null)
            {
                CachingImageResult cachingImageResult;
                foreach (Task<CachingImageResult> task in taskList)
                    cachingImageResult = await task;
            }
            _cachingImageCancellationTokenSource = null;
        }

        private async Task SynchronizeData()
        {
            try
            {
                ActionIsInProgress = true;

                if(_bodyExerciseList == null)
                    _bodyExerciseList = _bodyExerciseManager.FindBodyExercises();


                //Create BindingCollection
                GroupedTrainingExercises.Clear();

                if (_trainingDays != null)
                {
                    foreach (var trainingDay in _trainingDays)
                        PopulateBindingTrainingDay(trainingDay);
                }
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

        public ICommand CreateTrainingDayCommand
        {
            get
            {
                return new Command(async () => {
                    await CreateTrainingDay();
                });
            }
        }

        private async Task CreateTrainingDay()
        {
            if (ActionIsInProgress)
                return;

            try
            {
                ActionIsInProgress = true;

                if (_trainingDays != null)
                {
                    var newTrainingDay = new TrainingDay()
                    {
                        Year = Year,
                        WeekOfYear = WeekOfYear,
                        DayOfWeek = DayOfWeek,
                        TrainingDayId = 0,
                        UserId = UserId
                    };
                    if (await CreateTrainingDayViewModel.Show(newTrainingDay, this))
                    {
                        _trainingDays.Add(newTrainingDay);
                        //Binding trainingDay for refresh view 
                        PopulateBindingTrainingDay(newTrainingDay);
                    }
                }
            }
            catch (Exception exception)
            {
            }
            finally
            {
                ActionIsInProgress = false;
            }
        }

        public ICommand AddExerciseCommand
        {
            get
            {
                return new Command(async (trainingDayObject) => {
                    if (ActionIsInProgress)
                        return;

                    try
                    {
                        ActionIsInProgress = true;
                        var trainingDay = trainingDayObject as TrainingDay;
                        if (trainingDay != null)
                        {
                            var selectTrainingExercisesViewModelResut = await SelectTrainingExercisesViewModel.Show(this);
                            if (selectTrainingExercisesViewModelResut.Result && selectTrainingExercisesViewModelResut.BodyExerciseList != null)
                            {
                                if (trainingDay.TrainingExercises == null)
                                    trainingDay.TrainingExercises = new List<TrainingExercise>();

                                int nextIdTrainingExercise = 1;
                                if (trainingDay.TrainingExercises.Count > 0)
                                    nextIdTrainingExercise = trainingDay.TrainingExercises.Max(te => te.Id) + 1;
                                foreach (var bodyExercise in selectTrainingExercisesViewModelResut.BodyExerciseList)
                                {
                                    var trainingExercise = new TrainingExercise()
                                    {
                                        Year = trainingDay.Year,
                                        WeekOfYear = trainingDay.WeekOfYear,
                                        DayOfWeek = trainingDay.DayOfWeek,
                                        UserId = trainingDay.UserId,
                                        BodyExerciseId = bodyExercise.Id,
                                        Id = nextIdTrainingExercise
                                    };
                                    trainingDay.TrainingExercises.Add(trainingExercise);
                                    nextIdTrainingExercise++;
                                }
                                //Binding trainingDay for refresh view
                                await SynchronizeData(); // KAKA
                                //TODO synchronise with websevice
                                //PopulateBindingTrainingDay(trainingDay);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                    }
                    finally
                    {
                        ActionIsInProgress = false;
                    }
                });
            }
        }

        #region Binding Properties

        private string _createTrainingLabel;
        public string CreateTrainingLabel
        {
            get { return _createTrainingLabel; }
            set
            {
                _createTrainingLabel = value;
                OnPropertyChanged();
            }
        }

        private string _addExerciseLabel;
        public string AddExerciseLabel
        {
            get { return _addExerciseLabel; }
            set
            {
                _addExerciseLabel = value;
                OnPropertyChanged();
            }
        }

        private string _trainingModeLabel;
        public string TrainingModeLabel
        {
            get { return _trainingModeLabel; }
            set
            {
                _trainingModeLabel = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
