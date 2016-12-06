using Acr.UserDialogs;
using BodyReportMobile.Core.Data;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Framework.Binding;
using BodyReportMobile.Core.Message.Binding;
using BodyReportMobile.Core.WebServices;
using BodyReportMobile.Core.ServiceLayers;
using BodyReport.Message;
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
        private TrainingDayService _trainingDayService;
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
            _trainingDayService = new TrainingDayService(_dbContext);
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
            EditTrainingDayLabel = Translation.Get(TRS.EDIT);
            DeleteTrainingDayLabel = Translation.Get(TRS.DELETE);
            AddExerciseLabel = Translation.Get(TRS.ADD_EXERCISES);
            EditLabel = Translation.Get(TRS.EDIT);
            DeleteLabel = Translation.Get(TRS.DELETE);
        }

        private GenericGroupModelCollection<BindingTrainingExercise> PopulateBindingTrainingDay(TrainingDay trainingDay)
        {
            if (trainingDay == null)
                return null;
            
            string weightUnit = "kg";

            if (UserData.Instance.UserInfo.Unit == (int)TUnitType.Imperial)
                weightUnit = Translation.Get(TRS.POUND);

            StringBuilder setRepSb = new StringBuilder();
            StringBuilder setRepWeightSb = new StringBuilder();
            string formatSetRep = "{0} x {1}";
            string beginHourStr, endHourStr;
            BindingTrainingExercise bindingTrainingExercise;

            var collection = new GenericGroupModelCollection<BindingTrainingExercise>(); ;
            beginHourStr = trainingDay.BeginHour.ToLocalTime().ToString("HH:mm");
            endHourStr = trainingDay.EndHour.ToLocalTime().ToString("HH:mm");
            collection.LongName = string.Format("{0} {1} {2} {3}", Translation.Get(TRS.FROM), beginHourStr, Translation.Get(TRS.TO), endHourStr);
            collection.ShortName = collection.LongName;
            collection.ReferenceObject = trainingDay;
            collection.Clear();

            if (trainingDay.TrainingExercises != null)
            {
                BodyExercise bodyExercise;
                TrainingExercise trainingExercise;
                for (int i = 0; i < trainingDay.TrainingExercises.Count; i++)
                {
                    trainingExercise = trainingDay.TrainingExercises[i];
                    bodyExercise = _bodyExerciseList.Where(m => m.Id == trainingExercise.BodyExerciseId).FirstOrDefault();
                    bindingTrainingExercise = new BindingTrainingExercise()
                    {
                        TrainingExercise = trainingExercise,
                        BodyExerciseId = trainingExercise.BodyExerciseId,
                        RestTime = trainingExercise.RestTime,
                        UpOrderVisible = i != 0,
                        DownOrderVisible = i != (trainingDay.TrainingExercises.Count -1)
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
            }

            return collection;
        }
        
        private async Task CachingImagesAsync(List<BindingTrainingExercise> bindingGenericTrainingExercises)
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
                {
                    var bodyExerciseService = new BodyExerciseService(_dbContext);
                    _bodyExerciseList = bodyExerciseService.FindBodyExercises();
                }   
                
                //Create BindingCollection
                GroupedTrainingExercises.Clear();

                if (_trainingDays != null)
                {
                    foreach (var trainingDay in _trainingDays)
                        CreateOrReplaceBindingTrainingDay(trainingDay);
                }
            }
            catch (Exception except)
            {
                await _userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
        }

        private void CreateOrReplaceBindingTrainingDay(TrainingDay trainingDay)
        {
            var newGroupedTrainingExercises = PopulateBindingTrainingDay(trainingDay);
            
            var collection = GroupedTrainingExercises.Where(lgte => TrainingDayKey.IsEqualByKey((TrainingDay)lgte.ReferenceObject, trainingDay)).FirstOrDefault();
            if (collection == null)
            {
                collection = new GenericGroupModelCollection<BindingTrainingExercise>();
                GroupedTrainingExercises.Add(newGroupedTrainingExercises);
            }
            else
            {
                int indexOf = GroupedTrainingExercises.IndexOf(collection);
                if (indexOf == -1)
                    return;
                GroupedTrainingExercises[indexOf] = newGroupedTrainingExercises;
            }

            if (newGroupedTrainingExercises != null && newGroupedTrainingExercises.Count > 0)
            {
                List<BindingTrainingExercise> bindingList = new List<BindingTrainingExercise>();
                bindingList.AddRange(newGroupedTrainingExercises);
                Task t = CachingImagesAsync(bindingList);
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
                    if (await CreateTrainingDayViewModel.ShowAsync(newTrainingDay, TEditMode.Create, this))
                    {
                        _trainingDays.Add(newTrainingDay);
                        //Binding trainingDay for refresh view 
                        CreateOrReplaceBindingTrainingDay(newTrainingDay);
                    }
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to create training day", except);
            }
        }

        private async Task EditTrainingDayActionAsync(TrainingDay trainingDay)
        {
            if (trainingDay == null)
                return;
            try
            {
                if (await CreateTrainingDayViewModel.ShowAsync(trainingDay, TEditMode.Edit, this))
                {
                    //Binding trainingDay for refresh view 
                    CreateOrReplaceBindingTrainingDay(trainingDay);
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to edite training day", except);
                await _userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
        }

        private async Task DeleteTrainingDayActionAsync(TrainingDay trainingDay)
        {
            if (trainingDay == null)
                return;
            try
            {
                if (await _userDialog.ConfirmAsync(string.Format(Translation.Get(TRS.ARE_YOU_SURE_YOU_WANNA_DELETE_THIS_ELEMENT_PI), Translation.Get(TRS.TRAINING_DAY)),
                                                   Translation.Get(TRS.QUESTION), Translation.Get(TRS.YES), Translation.Get(TRS.NO)))
                {
                    //Delete on server
                    if (await TrainingDayWebService.DeleteTrainingDayAsync(trainingDay))
                    {
                        //Delete on local
                        _trainingDayService.DeleteTrainingDay(trainingDay);
                        //Binding trainingDay for refresh view 
                        _trainingDays.Remove(trainingDay);
                        var collection = GroupedTrainingExercises.Where(lgte => TrainingDayKey.IsEqualByKey((TrainingDay)lgte.ReferenceObject, trainingDay)).FirstOrDefault();
                        if (collection != null)
                            GroupedTrainingExercises.Remove(collection);
                    }
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to delete training day", except);
                await _userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
        }

        private async Task AddExerciseActionAsync(TrainingDay trainingDay)
        {
            if (trainingDay == null)
                return;
            try
            {
                int indexOfTrainingDay = _trainingDays.IndexOf(trainingDay);
                if (indexOfTrainingDay != -1)
                {
                    var selectTrainingExercisesViewModelResut = await SelectTrainingExercisesViewModel.ShowAsync(trainingDay, this, async (trainingDayKey, selectedBodyExerciseList) => {
                        // Validate by upload data on server
                        if (trainingDayKey != null && selectedBodyExerciseList != null)
                        {
                            var trainingDayScenario = new TrainingDayScenario() { ManageExercise = true };
                            var modifiedTrainingDay = await TrainingDayWebService.GetTrainingDayAsync(trainingDayKey, trainingDayScenario);

                            if (modifiedTrainingDay.TrainingExercises == null)
                            { // New training exercises
                                modifiedTrainingDay.TrainingExercises = new List<TrainingExercise>();
                            }

                            //AddExerciseActionAsync new exercises
                            int nextIdTrainingExercise = 1;
                            if (modifiedTrainingDay.TrainingExercises.Count > 0)
                                nextIdTrainingExercise = modifiedTrainingDay.TrainingExercises.Max(te => te.Id) + 1;
                            foreach (var bodyExercise in selectedBodyExerciseList)
                            {
                                var trainingExercise = new TrainingExercise()
                                {
                                    Year = modifiedTrainingDay.Year,
                                    WeekOfYear = modifiedTrainingDay.WeekOfYear,
                                    DayOfWeek = modifiedTrainingDay.DayOfWeek,
                                    UserId = modifiedTrainingDay.UserId,
                                    TrainingDayId = modifiedTrainingDay.TrainingDayId,
                                    BodyExerciseId = bodyExercise.Id,
                                    Id = nextIdTrainingExercise
                                };
                                modifiedTrainingDay.TrainingExercises.Add(trainingExercise);
                                nextIdTrainingExercise++;
                            }
                            //synchronise to server
                            modifiedTrainingDay = await TrainingDayWebService.UpdateTrainingDayAsync(modifiedTrainingDay, trainingDayScenario);
                            //local update
                            _trainingDayService.UpdateTrainingDay(modifiedTrainingDay, trainingDayScenario);
                            return true;
                        }
                        return false;
                    });
                    if (selectTrainingExercisesViewModelResut.Result)
                    {
                        //reload local data
                        var trainingDayScenario = new TrainingDayScenario() { ManageExercise = true };
                        trainingDay = _trainingDayService.GetTrainingDay(trainingDay, trainingDayScenario);
                        //Change modified trainingday in list of trainingdays
                        _trainingDays[indexOfTrainingDay] = trainingDay;
                        //Binding trainingDay for refresh view
                        CreateOrReplaceBindingTrainingDay(trainingDay);
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
            if (bindingTrainingExercise == null || bindingTrainingExercise.TrainingExercise == null)
                return;
            
            try
            {
                var trainingExercise = bindingTrainingExercise.TrainingExercise;
                var editTrainingExerciseViewModelResult = await EditTrainingExerciseViewModel.ShowAsync(trainingExercise, this);
                if (editTrainingExerciseViewModelResult != null && editTrainingExerciseViewModelResult.Result)
                {
                    var trainingDayKey = new TrainingDayKey()
                    {
                        UserId = trainingExercise.UserId,
                        Year = trainingExercise.Year,
                        WeekOfYear = trainingExercise.WeekOfYear,
                        DayOfWeek = trainingExercise.DayOfWeek,
                        TrainingDayId = trainingExercise.TrainingDayId
                    };
                    var trainingDay = _trainingDays.Where(t => TrainingDayKey.IsEqualByKey(t, trainingDayKey)).FirstOrDefault();
                    if (trainingDay != null)
                    {
                        var indexOfTrainingDay = _trainingDays.IndexOf(trainingDay);
                        //Reload local data
                        var trainingDayScenario = new TrainingDayScenario() { ManageExercise = true };
                        trainingDay = _trainingDayService.GetTrainingDay(trainingDayKey, trainingDayScenario);
                        //Update trainingDay in list
                        _trainingDays[indexOfTrainingDay] = trainingDay;
                        //Update UI
                        CreateOrReplaceBindingTrainingDay(trainingDay);
                    }
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to edit training exercise set", except);
                await _userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
        }

        private async Task DeleteActionAsync(BindingTrainingExercise bindingTrainingExercise)
        {
            if (bindingTrainingExercise == null || bindingTrainingExercise.TrainingExercise == null)
                return;

            try
            {
                if (await _userDialog.ConfirmAsync(string.Format(Translation.Get(TRS.ARE_YOU_SURE_YOU_WANNA_DELETE_THIS_ELEMENT_PI), Translation.Get(TRS.TRAINING_EXERCISE)),
                                                   Translation.Get(TRS.QUESTION), Translation.Get(TRS.YES), Translation.Get(TRS.NO)))
                {
                    var trainingExercise = bindingTrainingExercise.TrainingExercise;
                    // Delete TrainingExercise on server
                    await TrainingExerciseWebService.DeleteTrainingExerciseAsync(trainingExercise);
                    
                    // Delete TrainingExercise on local database
                    var trainingExerciseService = new TrainingExerciseService(_dbContext);
                    trainingExerciseService.DeleteTrainingExercise(trainingExercise);

                    //Remove trainingExercise in trainingDay
                    TrainingDay trainingDay = null;
                    foreach (var trainingDayTmp in _trainingDays)
                    {
                        if (trainingDayTmp.TrainingExercises == null)
                            continue;

                        if (trainingDayTmp.TrainingExercises.FirstOrDefault(t => t == trainingExercise) != null)
                        {
                            trainingDayTmp.TrainingExercises.Remove(trainingExercise);
                            trainingDay = trainingDayTmp;
                        }
                    }

                    //Refresh binding
                    if (GroupedTrainingExercises != null)
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

                    if (trainingDay != null)
                        PopulateBindingTrainingDay(trainingDay);
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to delete training exercise", except);
                await _userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
        }

        private async Task TrainingDayOrderActionAsync(BindingTrainingExercise bindingTrainingExercise, bool up)
        {
            if (bindingTrainingExercise == null || bindingTrainingExercise.TrainingExercise == null)
                return;

            try
            {
                var trainingExercise = bindingTrainingExercise.TrainingExercise;
                var trainingDayKey = new TrainingDayKey()
                {
                    UserId = trainingExercise.UserId,
                    Year = trainingExercise.Year,
                    WeekOfYear = trainingExercise.WeekOfYear,
                    DayOfWeek = trainingExercise.DayOfWeek,
                    TrainingDayId = trainingExercise.TrainingDayId
                };
                var trainingDay = _trainingDays.Where(t => TrainingDayKey.IsEqualByKey(t, trainingDayKey)).FirstOrDefault();
                var indexOfTrainingDay = _trainingDays.IndexOf(trainingDay);
                if (trainingDay != null)
                {
                    var indexOf = trainingDay.TrainingExercises.IndexOf(trainingExercise);
                    if (up && indexOf > 0)
                    { //up
                        trainingDay.TrainingExercises.Remove(trainingExercise);
                        trainingDay.TrainingExercises.Insert(indexOf - 1, trainingExercise);
                    }
                    if (!up && indexOf < (trainingDay.TrainingExercises.Count - 1))
                    { //down
                        trainingDay.TrainingExercises.Remove(trainingExercise);
                        trainingDay.TrainingExercises.Insert(indexOf + 1, trainingExercise);
                    }

                    indexOf = 1;
                    foreach (var trainingExerciseTmp in trainingDay.TrainingExercises)
                    {
                        trainingExerciseTmp.Id = indexOf;
                        indexOf++;
                    }

                    //Save in server
                    var trainingDayScenario = new TrainingDayScenario() { ManageExercise = true };
                    trainingDay = await TrainingDayWebService.UpdateTrainingDayAsync(trainingDay, trainingDayScenario);
                    //Save in local database
                    _trainingDayService.UpdateTrainingDay(trainingDay, trainingDayScenario);
                    //Update trainingDay in list
                    _trainingDays[indexOfTrainingDay] = trainingDay;
                    //Update UI
                    CreateOrReplaceBindingTrainingDay(trainingDay);
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to order exercise", except);
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

        private string _editTrainingDayLabel;
        public string EditTrainingDayLabel
        {
            get { return _editTrainingDayLabel; }
            set
            {
                _editTrainingDayLabel = value;
                OnPropertyChanged();
            }
        }

        private string _deleteTrainingDayLabel;
        public string DeleteTrainingDayLabel
        {
            get { return _deleteTrainingDayLabel; }
            set
            {
                _deleteTrainingDayLabel = value;
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

        private ICommand _editTrainingDayCommand = null;
        public ICommand EditTrainingDayCommand
        {
            get
            {
                if (_editTrainingDayCommand == null)
                {
                    _editTrainingDayCommand = new ViewModelCommandAsync(this, async (trainingDayObject) =>
                    {
                        await EditTrainingDayActionAsync(trainingDayObject as TrainingDay);
                    });
                }
                return _editTrainingDayCommand;
            }
        }

        private ICommand _deleteTrainingDayCommand = null;
        public ICommand DeleteTrainingDayCommand
        {
            get
            {
                if (_deleteTrainingDayCommand == null)
                {
                    _deleteTrainingDayCommand = new ViewModelCommandAsync(this, async (trainingDayObject) =>
                    {
                        await DeleteTrainingDayActionAsync(trainingDayObject as TrainingDay);
                    });
                }
                return _deleteTrainingDayCommand;
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

        private ICommand _upTrainingDayOrderCommand = null;
        public ICommand UpTrainingDayOrderCommand
        {
            get
            {
                if (_upTrainingDayOrderCommand == null)
                {
                    _upTrainingDayOrderCommand = new ViewModelCommandAsync(this, async (bindingTrainingExerciseObject) =>
                    {
                        await TrainingDayOrderActionAsync(bindingTrainingExerciseObject as BindingTrainingExercise, true);
                    });
                }
                return _upTrainingDayOrderCommand;
            }
        }

        private ICommand _downTrainingDayOrderCommand = null;
        public ICommand DownTrainingDayOrderCommand
        {
            get
            {
                if (_downTrainingDayOrderCommand == null)
                {
                    _downTrainingDayOrderCommand = new ViewModelCommandAsync(this, async (bindingTrainingExerciseObject) =>
                    {
                        await TrainingDayOrderActionAsync(bindingTrainingExerciseObject as BindingTrainingExercise, false);
                    });
                }
                return _downTrainingDayOrderCommand;
            }
        }

        #endregion
    }
}
