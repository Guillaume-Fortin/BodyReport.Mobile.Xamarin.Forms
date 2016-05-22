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
			CreateTrainingLabel = Translation.Get(TRS.CREATE); //necessary for ios Toolbaritem binding failed
        }

        protected override void Closed(bool backPressed)
        {
            base.Closed(backPressed);
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

        protected override async Task ShowAsync()
        {
            await base.ShowAsync();

            try
            {
                ActionIsInProgress = true;
                if (_trainingDays != null && _trainingDays.Count > 0)
                {
                    var trainingDay = _trainingDays[0];
                    UserId = trainingDay.UserId;
                    Year = trainingDay.Year;
                    WeekOfYear = trainingDay.WeekOfYear;
                    DayOfWeek = trainingDay.DayOfWeek;
                }
                await SynchronizeDataAsync();
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to Show TrainingDayViewModel", except);
            }
            finally
            {
                ActionIsInProgress = false;
            }
        }

        public static async Task<TrainingDayViewModelResut> ShowAsync(List<TrainingDay> trainingDayList, BaseViewModel parent = null)
        {
            TrainingDayViewModelResut trainingDayViewModelResut = new TrainingDayViewModelResut();
            if (trainingDayList != null && trainingDayList.Count > 0)
            {
                var viewModel = new TrainingDayViewModel();
                viewModel._trainingDays = trainingDayList;
                await ShowModalViewModelAsync(viewModel, parent);

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
			CreateTrainingLabel = Translation.Get(TRS.CREATE);
            TrainingModeLabel = "Training Mode";
            AddExerciseLabel = Translation.Get(TRS.ADD_EXERCISES);
            EditLabel = Translation.Get(TRS.EDIT);
            DeleteLabel = Translation.Get(TRS.DELETE);
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
                        TrainingExercise = trainingExercise,
                        BodyExerciseId = trainingExercise.BodyExerciseId,
                        RestTime = trainingExercise.RestTime
                    };
                    bindingTrainingExercise.BodyExerciseName = bodyExercise != null ? bodyExercise.Name : Translation.Get(TRS.UNKNOWN);
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
                    List<BindingTrainingExercise> bindingList = new List<BindingTrainingExercise>();
                    bindingList.AddRange(collection);
                    Task t = CachingImagesAsync(bindingList);
                }
            }
            
            foreach (var trainingExercise in localGroupedTrainingExercises)
            {
                GroupedTrainingExercises.Add(trainingExercise);
            }
        }
        
        public async Task CachingImagesAsync(List<BindingTrainingExercise> bindingGenericTrainingExercises)
        {
            if (bindingGenericTrainingExercises == null)
                return;

            lock (_locker)
            {
                if (_cachingImageCancellationTokenSource == null)
                    _cachingImageCancellationTokenSource = new CancellationTokenSource();
            }
            
            string imageName, urlImage, localImagePath;
            string urlImages = HttpConnector.Instance.BaseUrl + "images/bodyexercises/{0}";
            List<Task> taskList = null;
            foreach (var bindingTrainingExercise in bindingGenericTrainingExercises)
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
                    var t = AppTools.Instance.CachingImageAsync<BindingTrainingExercise>(bindingTrainingExercise, urlImage, localImagePath, OnCachingImageResult);
                    if(t != null)
                        taskList.Add(t);
                }
            }

            if (taskList != null)
            {
                foreach (Task task in taskList)
                    await task;
            }
            _cachingImageCancellationTokenSource = null;
        }

        private void OnCachingImageResult(CachingImageResult<BindingTrainingExercise> result)
        {
            if (result != null && result.BindingObject != null)
            {
                result.BindingObject.Image = result.ImagePath;
            }
        }

        private async Task SynchronizeDataAsync()
        {
            try
            {
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
        }

        private async Task CreateTrainingDayActionAsync()
        {
            try
            {
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
                    if (await CreateTrainingDayViewModel.ShowAsync(newTrainingDay, this))
                    {
                        _trainingDays.Add(newTrainingDay);
                        //Binding trainingDay for refresh view 
                        PopulateBindingTrainingDay(newTrainingDay);
                    }
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to create training day", except);
            }
        }

        private async Task AddExerciseActionAsync(TrainingDay trainingDay)
        {
            try
            {
                if (trainingDay != null)
                {
                    var selectTrainingExercisesViewModelResut = await SelectTrainingExercisesViewModel.ShowAsync(this);
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
                                TrainingDayId = trainingDay.TrainingDayId,
                                BodyExerciseId = bodyExercise.Id,
                                Id = nextIdTrainingExercise
                            };
                            trainingDay.TrainingExercises.Add(trainingExercise);
                            nextIdTrainingExercise++;
                        }
                        //Binding trainingDay for refresh view
                        await SynchronizeDataAsync(); // KAKA
                        //synchronise with webservice
                        await TrainingDayWebService.UpdateTrainingDayAsync(trainingDay);
                        //Todo replace trainingDay by http response trainingDay (by security)
                    }
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to add exercise", except);
                await _userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
        }

        private async Task EditActionAsync(BindingTrainingExercise bindingTrainingExercise)
        {
           // if (bindingTrainingExercise == null)
             //   return;
            throw new NotImplementedException();
        }

        private async Task DeleteActionAsync(BindingTrainingExercise bindingTrainingExercise)
        {
            if (bindingTrainingExercise == null)
                return;

            try
            {
                if (await _userDialog.ConfirmAsync(string.Format(Translation.Get(TRS.ARE_YOU_SURE_YOU_WANNA_DELETE_THIS_ELEMENT_PI), Translation.Get(TRS.TRAINING_EXERCISE)),
                                                   Translation.Get(TRS.QUESTION), Translation.Get(TRS.YES), Translation.Get(TRS.NO)))
                {
                    // Delete TrainingExercise on server
                    await TrainingExerciseWebService.DeleteTrainingExerciseAsync(bindingTrainingExercise.TrainingExercise);

                    // Delete TrainingExercise on local database (Futur use)
                    var trainingExerciseManager = new TrainingExerciseManager(_dbContext);
                    trainingExerciseManager.DeleteTrainingExercise(bindingTrainingExercise.TrainingExercise);

                    //Refresh binding
                    if(GroupedTrainingExercises != null)
                    {
                        foreach(var gte in GroupedTrainingExercises)
                        {
                            if (gte.Contains(bindingTrainingExercise))
                            {
                                gte.Remove(bindingTrainingExercise);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to add exercise", except);
                await _userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
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

        private string _editLabel;
        public string EditLabel
        {
            get { return _editLabel; }
            set
            {
                _editLabel = value;
                OnPropertyChanged();
            }
        }

        private string _deleteLabel;
        public string DeleteLabel
        {
            get { return _deleteLabel; }
            set
            {
                _deleteLabel = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Command

        private ICommand _createTrainingDayCommand = null;
        public ICommand CreateTrainingDayCommand
        {
            get
            {
                if (_createTrainingDayCommand == null)
                {
                    _createTrainingDayCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await CreateTrainingDayActionAsync();
                    });
                }
                return _createTrainingDayCommand;
            }
        }
        
        private ICommand _addExerciseCommand = null;
        public ICommand AddExerciseCommand
        {
            get
            {
                if (_addExerciseCommand == null)
                {
                    _addExerciseCommand = new ViewModelCommandAsync(this, async (trainingDayObject) =>
                    {
                        await AddExerciseActionAsync(trainingDayObject as TrainingDay);
                    });
                }
                return _addExerciseCommand;
            }
        }

        private ICommand _editCommand = null;
        public ICommand EditCommand
        {
            get
            {
                if (_editCommand == null)
                {
                    _editCommand = new ViewModelCommandAsync(this, async (bindingTrainingExerciseObject) =>
                    {
                        await EditActionAsync(bindingTrainingExerciseObject as BindingTrainingExercise);
                    });
                }
                return _editCommand;
            }
        }

        private ICommand _deleteCommand = null;
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new ViewModelCommandAsync(this, async (bindingTrainingExerciseObject) =>
                    {
                        await DeleteActionAsync(bindingTrainingExerciseObject as BindingTrainingExercise);
                    });
                }
                return _deleteCommand;
            }
        }

        #endregion
    }
}
