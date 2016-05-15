using Acr.UserDialogs;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Message.Binding;
using BodyReportMobile.Core.ServiceManagers;
using BodyReportMobile.Core.ViewModels.Generic;
using Message;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using XLabs.Ioc;

namespace BodyReportMobile.Core.ViewModels
{
    public class SelectTrainingExercisesViewModelResut
    {
        public bool Result = false;
        public List<BodyExercise> BodyExerciseList { get; set; } = new List<BodyExercise>();
    }

    public class SelectTrainingExercisesViewModel : BaseViewModel
    {
        private SQLiteConnection _dbContext;
        private MuscularGroupManager _muscularGroupManager;
        private MuscleManager _muscleManager;
        private BodyExerciseManager _bodyExerciseManager;
        private IUserDialogs _userDialog;

        public List<BindingBodyExercise> BindingBodyExercises { get; set; } = new List<BindingBodyExercise>();
        public List<BodyExercise> SelectedBodyExerciseList = new List<BodyExercise>();
        private List<MuscularGroup> _muscularGroups;
        private List<Muscle> _muscles;
        private List<BodyExercise> _bodyExercises;

        private object _locker = new object();
        private CancellationTokenSource _cachingImageCancellationTokenSource = null;

        public SelectTrainingExercisesViewModel() : base()
        {
            ShowDelayInMs = 0;
            _dbContext = Resolver.Resolve<ISQLite>().GetConnection();
            _bodyExerciseManager = new BodyExerciseManager(_dbContext);
            _muscularGroupManager = new MuscularGroupManager(_dbContext);
            _muscleManager = new MuscleManager(_dbContext);
            _userDialog = Resolver.Resolve<IUserDialogs>();
        }

        protected override async Task ShowAsync()
        {
            await base.ShowAsync();

            ActionIsInProgress = true;
            await SynchronizeDataAsync();
            ActionIsInProgress = false;
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

        public static async Task<SelectTrainingExercisesViewModelResut> ShowAsync(BaseViewModel parent = null)
        {
            var selectTrainingExercisesViewModelResut = new SelectTrainingExercisesViewModelResut();
            var viewModel = new SelectTrainingExercisesViewModel();
            if(await ShowModalViewModelAsync(viewModel, parent))
            {
                selectTrainingExercisesViewModelResut.Result = true;
                selectTrainingExercisesViewModelResut.BodyExerciseList = viewModel.SelectedBodyExerciseList;
            }

            return selectTrainingExercisesViewModelResut;
        }

        protected override void InitTranslation()
        {
            base.InitTranslation();
            TitleLabel = Translation.Get(TRS.TRAINING_EXERCISE);
            MuscularGroupLabel = Translation.Get(TRS.MUSCULAR_GROUP);
            MuscleLabel = Translation.Get(TRS.MUSCLE);
            AddExercisesLabel = Translation.Get(TRS.ADD_EXERCISES);
            BodyExerciseLabel = Translation.Get(TRS.BODY_EXERCISES);
            ValidateLabel = Translation.Get(TRS.VALIDATE);
        }

        private async Task SynchronizeDataAsync(bool byPassCurrentAction=false)
        {
            try
            {
                if(_muscularGroups == null)
                    _muscularGroups = _muscularGroupManager.FindMuscularGroups();

                if(_muscles == null)
                    _muscles = _muscleManager.FindMuscles();

                if (_bodyExercises == null)
                    _bodyExercises = _bodyExerciseManager.FindBodyExercises();

                /*
                if(onShow && _muscularGroups != null && _muscularGroups.Count > 0 && _muscles != null)
                {
                    MuscularGroup = _muscularGroups[0];
                    Muscle = _muscles.Where(m => m.MuscularGroupId == MuscularGroup.Id).FirstOrDefault();
                }*/

                //Refresh BodyExercise
                BindingBodyExercises.Clear();
                if(MuscularGroup != null && Muscle != null && _bodyExercises != null && _bodyExercises.Count > 0)
                {
                    BindingBodyExercise bindingBodyExercise;
                    var bodyexerciseList = _bodyExercises.Where(be => be.MuscleId == Muscle.Id);
                    foreach (var bodyexercise in bodyexerciseList)
                    {
                        bindingBodyExercise = new BindingBodyExercise()
                        {
                            BodyExercise = bodyexercise,
                            Name = bodyexercise.Name
                        };
                        BindingBodyExercises.Add(bindingBodyExercise);
                    }
                    
                    if (BindingBodyExercises.Count > 0)
                    {
                        List<BindingBodyExercise> bindingList = new List<BindingBodyExercise>();
                        bindingList.AddRange(BindingBodyExercises);
                        Task t = CachingImagesAsync(bindingList);
                    }
                }
                OnPropertyChanged("BindingBodyExercises");
            }
            catch (Exception except)
            {
                await _userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
        }

        public async Task CachingImagesAsync(List<BindingBodyExercise> bindingBodyExerciseList)
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

            string urlImage, localImagePath;
            string urlImages = HttpConnector.Instance.BaseUrl + "images/bodyexercises/{0}";
            List<Task> taskList = null;
            foreach (var bindingBodyExercise in bindingBodyExerciseList)
            {
                if (_cachingImageCancellationTokenSource.Token.IsCancellationRequested)
                    _cachingImageCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (taskList == null)
                    taskList = new List<Task>();

                if (bindingBodyExercise.BodyExercise != null && !string.IsNullOrWhiteSpace(bindingBodyExercise.BodyExercise.ImageName))
                {
                    urlImage = string.Format(urlImages, bindingBodyExercise.BodyExercise.ImageName);
                    localImagePath = Path.Combine(AppTools.BodyExercisesImagesDirectory, bindingBodyExercise.BodyExercise.ImageName);
                    var t = AppTools.Instance.CachingImageAsync<BindingBodyExercise>(bindingBodyExercise, urlImage, localImagePath, OnCachingImageResult);
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

        private void OnCachingImageResult(CachingImageResult<BindingBodyExercise> result)
        {
            if (result != null && result.BindingObject != null)
            {
                result.BindingObject.Image = result.ImagePath;
            }
        }

        private async Task SelectMuscularGroupActionAsync()
        {
            try
            {
                if (_muscularGroups != null && _muscularGroups.Count > 0)
                {
                    _muscularGroups = _muscularGroups.OrderBy(m => m.Name).ToList();

                    Message.GenericData data, currentData;
                    currentData = null;
                    var datas = new List<Message.GenericData>();
                    foreach (var muscularGroup in _muscularGroups)
                    {
                        data = new Message.GenericData() { Tag = muscularGroup, Name = muscularGroup.Name };
                        datas.Add(data);

                        if (muscularGroup == MuscularGroup)
                            currentData = data;
                    }

                    var result = await ListViewModel.ShowGenericListAsync(Translation.Get(TRS.MUSCULAR_GROUP), datas, currentData, this);

                    if (result.Validated && result.SelectedData != null)
                    {
                        MuscularGroup = result.SelectedData.Tag as MuscularGroup;
                        Muscle = null;
                        await SynchronizeDataAsync();
                        await SelectMuscleActionAsync();
                    }
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to select muscular group", except);
            }
        }
        
        private async Task SelectMuscleActionAsync()
        {
            try
            {
                if (MuscularGroup == null)
                {
                    await _userDialog.AlertAsync("Select first one muscular group", Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
                }
                else
                {
                    List<Muscle> muscleList = null;
                    if (_muscles != null)
                        muscleList = _muscles.Where(m => m.MuscularGroupId == MuscularGroup.Id).OrderBy(m => m.Name).ToList();
                    if (muscleList != null && muscleList.Count > 0)
                    {
                        Message.GenericData data, currentData;
                        currentData = null;
                        var datas = new List<Message.GenericData>();
                        foreach (var muscle in muscleList)
                        {
                            data = new Message.GenericData() { Tag = muscle, Name = muscle.Name };
                            datas.Add(data);

                            if (muscle == Muscle)
                                currentData = data;
                        }

                        var result = await ListViewModel.ShowGenericListAsync(Translation.Get(TRS.MUSCLE), datas, currentData, this);

                        if (result.Validated && result.SelectedData != null)
                        {
                            Muscle = result.SelectedData.Tag as Muscle;
                            await SynchronizeDataAsync();
                        }
                    }
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to select muscle", except);
            }
        }

        private void SelectBodyExerciseAction(BindingBodyExercise BindingBodyExercise)
        {
            try
            {
                if (BindingBodyExercise != null)
                    BindingBodyExercise.Selected = !BindingBodyExercise.Selected;
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to select body exercise", except);
            }
        }

        private async Task ValidateActionAsync()
        {
            try
            {
                if(BindingBodyExercises != null && BindingBodyExercises.Count > 0)
                {
                    var bodyExerciseSelectedList = BindingBodyExercises.Where(be => be.Selected).Select(be => be.BodyExercise).ToList();
                    if(bodyExerciseSelectedList == null || bodyExerciseSelectedList.Count == 0)
                        await _userDialog.AlertAsync("Select one or more body exercise", Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
                    else
                    {
                        SelectedBodyExerciseList.Clear();
                        SelectedBodyExerciseList.AddRange(bodyExerciseSelectedList);
                        CloseViewModel();
                    }
                }
            }
            catch (Exception except)
            {
                await _userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
        }

        #region Binding Properties

        private string _validateLabel;
        public string ValidateLabel
        {
            get { return _validateLabel; }
            set
            {
                _validateLabel = value;
                OnPropertyChanged();
            }
        }

        private string _muscularGroupLabel;
        public string MuscularGroupLabel
        {
            get { return _muscularGroupLabel; }
            set
            {
                _muscularGroupLabel = value;
                OnPropertyChanged();
            }
        }

        private string _muscleLabel;
        public string MuscleLabel
        {
            get { return _muscleLabel; }
            set
            {
                _muscleLabel = value;
                OnPropertyChanged();
            }
        }

        private string _addExercisesLabel;
        public string AddExercisesLabel
        {
            get { return _addExercisesLabel; }
            set
            {
                _addExercisesLabel = value;
                OnPropertyChanged();
            }
        }

        private string _bodyExerciseLabel;
        public string BodyExerciseLabel
        {
            get { return _bodyExerciseLabel; }
            set
            {
                _bodyExerciseLabel = value;
                OnPropertyChanged();
            }
        }

        private MuscularGroup _muscularGroup;
        public MuscularGroup MuscularGroup
        {
            get { return _muscularGroup; }
            set
            {
                _muscularGroup = value;
                OnPropertyChanged();
            }
        }

        private Muscle _mucle;
        public Muscle Muscle
        {
            get { return _mucle; }
            set
            {
                _mucle = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Command

        private ICommand _selectMuscularGroupCommand = null;
        public ICommand SelectMuscularGroupCommand
        {
            get
            {
                if (_selectMuscularGroupCommand == null)
                {
                    _selectMuscularGroupCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await SelectMuscularGroupActionAsync();
                    });
                }

                return _selectMuscularGroupCommand;
            }
        }

        private ICommand _selectMuscleCommand = null;
        public ICommand SelectMuscleCommand
        {
            get
            {
                if (_selectMuscleCommand == null)
                {
                    _selectMuscleCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await SelectMuscleActionAsync();
                    });
                }

                return _selectMuscleCommand;
            }
        }

        private ICommand _selectBodyExerciseCommand = null;
        public ICommand SelectBodyExerciseCommand
        {
            get
            {
                if (_selectBodyExerciseCommand == null)
                {
                    _selectBodyExerciseCommand = new ViewModelCommand(this, (BindingBodyExerciseObject) =>
                    {
                        var bindingBodyExercise = BindingBodyExerciseObject as BindingBodyExercise;
                        SelectBodyExerciseAction(bindingBodyExercise);
                    });
                }

                return _selectBodyExerciseCommand;
            }
        }

        private ICommand _validateCommand = null;
        public ICommand ValidateCommand
        {
            get
            {
                if (_validateCommand == null)
                {
                    _validateCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await ValidateActionAsync();
                    });
                }

                return _validateCommand;
            }
        }

        #endregion
    }
}
