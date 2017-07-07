using Acr.UserDialogs;
using BodyReport.Message;
using BodyReportMobile.Core.Crud.Transformer;
using BodyReportMobile.Core.Data;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Message.Binding;
using BodyReportMobile.Core.ServiceLayers;
using BodyReportMobile.Core.WebServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using XLabs.Ioc;

namespace BodyReportMobile.Core.ViewModels
{
    public class EditTrainingExerciseViewModelResult
    {
        public bool Result = false;
    }

    public class EditTrainingExerciseViewModel : BaseViewModel
    {
        private ApplicationDbContext _dbContext;
        private UserInfo _userInfo;
        private TrainingExercise _trainingExercise;
        TrainingDayService _trainingDayService;
        private IUserDialogs _userDialog;

        public EditTrainingExerciseViewModel() : base()
        {
            DbContext = Resolver.Resolve<ISQLite>().GetConnection();
            _trainingDayService = new TrainingDayService(DbContext);
            _userDialog = Resolver.Resolve<IUserDialogs>();
        }

        public static async Task<EditTrainingExerciseViewModelResult> ShowAsync(TrainingExercise trainingExercise, BaseViewModel parent = null)
        {
            
            var viewModel = new EditTrainingExerciseViewModel();
            viewModel._trainingExercise = trainingExercise;
            var result = await ShowModalViewModelAsync(viewModel, parent);

            var editTrainingExerciseViewModelResult = new EditTrainingExerciseViewModelResult();
            editTrainingExerciseViewModelResult.Result = result;
            return editTrainingExerciseViewModelResult;
        }

        protected override async Task ShowAsync()
        {
            await base.ShowAsync();

            try
            {
                ActionIsInProgress = true;
                await SynchronizeDataAsync();
            }
            catch
            {}
            finally
            {
                ActionIsInProgress = false;
            }
        }

        protected override void InitTranslation()
        {
            base.InitTranslation();
            TitleLabel = Translation.Get(TRS.TRAINING_EXERCISE);
            ExerciseTitleLabel = Translation.Get(TRS.NAME);
            RestTimeLabel = Translation.Get(TRS.REST_TIME) + " (sec)";
            EccentricContractionTempoLabel = Translation.Get(TRS.ECCENTRIC_CONTRACTION);
            StretchPositionTempoLabel = Translation.Get(TRS.STRETCH_POSITION);
            ConcentricContractionTempoLabel = Translation.Get(TRS.CONCENTRIC_CONTRACTION);
            ContractedPositionTempoLabel = Translation.Get(TRS.CONTRACTED_POSITION);
            AddRepsLabel = Translation.Get(TRS.ADD_REPS);
            ValidateLabel = Translation.Get(TRS.VALIDATE);
            UnitLabel = Translation.Get(TRS.EXERCISE_UNIT_TYPE);
        }

        private TUnitType GetExerciseUnit()
        {
            TUnitType unit = TUnitType.Metric;
            if (_trainingExercise != null)
            {
                var trainingDayKey = new TrainingDayKey()
                {
                    UserId = _trainingExercise.UserId,
                    Year = _trainingExercise.Year,
                    WeekOfYear = _trainingExercise.WeekOfYear,
                    DayOfWeek = _trainingExercise.DayOfWeek,
                    TrainingDayId = _trainingExercise.TrainingDayId
                };
                var trainingDayScenario = new TrainingDayScenario() { ManageExercise = false };
                var trainingDay = _trainingDayService.GetTrainingDay(trainingDayKey, trainingDayScenario);
                if (trainingDay != null)
                    unit = trainingDay.Unit;
            }
            return unit;
        }

        private async Task SynchronizeDataAsync()
        {
            var unit = GetExerciseUnit();
            string weightUnit = "kg";
            if (unit == TUnitType.Imperial)
            {
                weightUnit = Translation.Get(TRS.POUND);
            }

            ExerciseTitle = Translation.GetInDB(BodyExerciseTransformer.GetTranslationKey(_trainingExercise.BodyExerciseId));
            RestTime = _trainingExercise.RestTime;
            EccentricContractionTempo = _trainingExercise.EccentricContractionTempo;
            StretchPositionTempo = _trainingExercise.StretchPositionTempo;
            ConcentricContractionTempo = _trainingExercise.ConcentricContractionTempo;
            ContractedPositionTempo = _trainingExercise.ContractedPositionTempo;

            string urlImages = HttpConnector.Instance.BaseUrl + "images/bodyexercises/{0}";
            var imageName = string.Format("{0}.png", _trainingExercise.BodyExerciseId);
            var urlImage = string.Format(urlImages, imageName);
            var localImagePath = Path.Combine(AppTools.BodyExercisesImagesDirectory, imageName);
            await AppTools.Instance.CachingImageAsync(_trainingExercise, urlImage, localImagePath, null);
            ExerciseImageSource = localImagePath;

            if (BindingTrainingExerciseSetReps == null)
                BindingTrainingExerciseSetReps = new ObservableCollection<BindingTrainingExerciseSetRep>();
            else
                BindingTrainingExerciseSetReps.Clear();

            if (_trainingExercise != null)
            {
                BindingTrainingExerciseSetRep bindingSetRep;
                if (_trainingExercise.TrainingExerciseSets == null || _trainingExercise.TrainingExerciseSets.Count == 0)
                {
                    bindingSetRep = new BindingTrainingExerciseSetRep();
                    bindingSetRep.RepsLabel = _trainingExercise.ExerciseUnitType == TExerciseUnitType.RepetitionNumber ? Translation.Get(TRS.REPS) : ($"{Translation.Get(TRS.EXECUTION_TIME)} (sec)");
                    bindingSetRep.WeightsLabel = $"{Translation.Get(TRS.WEIGHT)} ({weightUnit})";
                    bindingSetRep.RepsOrExecTimes = _trainingExercise.ExerciseUnitType == TExerciseUnitType.RepetitionNumber ? 8 : 30;
                    bindingSetRep.Weights = 0;
                    BindingTrainingExerciseSetReps.Add(bindingSetRep);
                }
                else
                {
                    int count = 0;
                    foreach (var trainingExerciseSet in _trainingExercise.TrainingExerciseSets)
                    {
                        for (int i = 0; i < trainingExerciseSet.NumberOfSets; i++)
                        {
                            bindingSetRep = new BindingTrainingExerciseSetRep();
                            if (count == 0)
                            {
                                bindingSetRep.RepsLabel = _trainingExercise.ExerciseUnitType == TExerciseUnitType.RepetitionNumber ? Translation.Get(TRS.REPS) : ($"{Translation.Get(TRS.EXECUTION_TIME)} (sec)");
                                bindingSetRep.WeightsLabel = $"{Translation.Get(TRS.WEIGHT)} ({weightUnit})";
                            }
                            else
                                bindingSetRep.RepsLabel = bindingSetRep.WeightsLabel = string.Empty; // necessary for trigger Text.Length
                            bindingSetRep.RepsOrExecTimes = _trainingExercise.ExerciseUnitType == TExerciseUnitType.RepetitionNumber ? trainingExerciseSet.NumberOfReps : trainingExerciseSet.ExecutionTime;
                            bindingSetRep.Weights = trainingExerciseSet.Weight;
                            BindingTrainingExerciseSetReps.Add(bindingSetRep);
                            count++;
                        }
                    }
                }
                if (BindingTrainingExerciseSetReps.Count > 0)
                    BindingTrainingExerciseSetReps[BindingTrainingExerciseSetReps.Count - 1].BtnPlusVisible = true;
            }
        }

        private void AddRepAction()
        {
            try
            {
                int previousRepOrExecTime = 0;
                double previousWeight = 0;
                if (BindingTrainingExerciseSetReps.Count > 0)
                {
                    previousRepOrExecTime = BindingTrainingExerciseSetReps[BindingTrainingExerciseSetReps.Count-1].RepsOrExecTimes;
                    previousWeight = BindingTrainingExerciseSetReps[BindingTrainingExerciseSetReps.Count-1].Weights;
                }

                foreach (var bindingSetRep in BindingTrainingExerciseSetReps)
                {
                    if (bindingSetRep.BtnPlusVisible)
                        bindingSetRep.BtnPlusVisible = false;
                }

                BindingTrainingExerciseSetReps.Add(new BindingTrainingExerciseSetRep()
                {
                    RepsOrExecTimes = previousRepOrExecTime,
                    Weights = previousWeight,
                    BtnPlusVisible = true,
                    RepsLabel = string.Empty,// necessary for trigger Text.Length
                    WeightsLabel = string.Empty // necessary for trigger Text.Length
                });

                
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to add rep in training exercise", except);
            }
        }

        private void DeleteRepAction(BindingTrainingExerciseSetRep bindingTrainingExerciseSetRep)
        {
            if (bindingTrainingExerciseSetRep == null)
                return;
            try
            {
                if (BindingTrainingExerciseSetReps.Count > 0 && BindingTrainingExerciseSetReps.IndexOf(bindingTrainingExerciseSetRep) != -1)
                {
                    BindingTrainingExerciseSetReps.Remove(bindingTrainingExerciseSetRep);
                    if (BindingTrainingExerciseSetReps.Count > 0)
                    {
                        BindingTrainingExerciseSetReps[BindingTrainingExerciseSetReps.Count-1].BtnPlusVisible = true;
                    }
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to supress rep in training exercise", except);
            }
        }

        private async Task ValidateActionAsync()
        {
            try
            {
                if(BindingTrainingExerciseSetReps != null && BindingTrainingExerciseSetReps.Count > 0 &&
                   _trainingExercise != null)
                {
                    var trainingExercise = _trainingExercise.Clone();
                    if (trainingExercise.TrainingExerciseSets == null)
                        trainingExercise.TrainingExerciseSets = new List<TrainingExerciseSet>();
                    else
                        trainingExercise.TrainingExerciseSets.Clear(); // empty sets for replacing

                    trainingExercise.RestTime = RestTime; // don't forget restime...
                    trainingExercise.EccentricContractionTempo = EccentricContractionTempo;
                    trainingExercise.StretchPositionTempo = StretchPositionTempo;
                    trainingExercise.ConcentricContractionTempo = ConcentricContractionTempo;
                    trainingExercise.ContractedPositionTempo = ContractedPositionTempo;
                    int nbSet = 0, currentRepOrExecTimeValue = 0;
                    var tupleSetRepList = new List<Tuple<int, int, double>>();
                    int repOrExecTimeValue;
                    double weightValue, currentWeightValue = 0;
                    for (int i = 0; i < BindingTrainingExerciseSetReps.Count; i++)
                    {
                        repOrExecTimeValue = BindingTrainingExerciseSetReps[i].RepsOrExecTimes;
                        weightValue = BindingTrainingExerciseSetReps[i].Weights;
                        if (repOrExecTimeValue == 0)
                            continue;

                        if (weightValue == currentWeightValue && repOrExecTimeValue == currentRepOrExecTimeValue)
                            nbSet++;
                        else
                        {
                            if (nbSet != 0)
                                tupleSetRepList.Add(new Tuple<int, int, double>(nbSet, currentRepOrExecTimeValue, currentWeightValue));
                            currentRepOrExecTimeValue = repOrExecTimeValue;
                            currentWeightValue = weightValue;
                            nbSet = 1;
                        }
                    }

                    //last data
                    if (nbSet != 0)
                        tupleSetRepList.Add(new Tuple<int, int, double>(nbSet, currentRepOrExecTimeValue, currentWeightValue));
                    
                    int id = 1;
                    foreach (Tuple<int, int, double> tupleSetRep in tupleSetRepList)
                    {
                        trainingExercise.TrainingExerciseSets.Add(new TrainingExerciseSet()
                        {
                            UserId = trainingExercise.UserId,
                            Year = trainingExercise.Year,
                            WeekOfYear = trainingExercise.WeekOfYear,
                            DayOfWeek = trainingExercise.DayOfWeek,
                            TrainingDayId = trainingExercise.TrainingDayId,
                            TrainingExerciseId = trainingExercise.Id,
                            Id = id,
                            NumberOfSets = tupleSetRep.Item1,
                            NumberOfReps = (trainingExercise.ExerciseUnitType == TExerciseUnitType.RepetitionNumber) ? tupleSetRep.Item2 : 0,
                            ExecutionTime = (trainingExercise.ExerciseUnitType == TExerciseUnitType.Time) ? tupleSetRep.Item2 : 0,
                            Weight = tupleSetRep.Item3
                        });
                        id++;
                    }

                    //Save in server
                    var trainingDayKey = new TrainingDayKey()
                    {
                        UserId = trainingExercise.UserId,
                        Year = trainingExercise.Year,
                        WeekOfYear = trainingExercise.WeekOfYear,
                        DayOfWeek = trainingExercise.DayOfWeek,
                        TrainingDayId = trainingExercise.TrainingDayId
                    };
                    var trainingDayScenario = new TrainingDayScenario() { ManageExercise = true };
                    var trainingDay = await TrainingDayWebService.GetTrainingDayAsync(trainingDayKey, trainingDayScenario);

                    //modify datas
                    var trainingExerciseTmp = trainingDay.TrainingExercises.Where(t => TrainingExerciseKey.IsEqualByKey(t, trainingExercise)).FirstOrDefault();
                    var indexOf = trainingDay.TrainingExercises.IndexOf(trainingExerciseTmp);
                    if (indexOf != -1)
                    {
                        //Replace exercise and sets
                        trainingDay.TrainingExercises[indexOf] = trainingExercise;
                        //update to server
                        trainingDay = await TrainingDayWebService.UpdateTrainingDayAsync(trainingDay, trainingDayScenario);
                        //Save modified data in local database
                        _trainingDayService.UpdateTrainingDay(trainingDay, trainingDayScenario);

                        CloseViewModel();
                    }
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to validate training rep/set", except);
                await _userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
        }

        private async Task ChangeExerciseUnitCommandAction()
        {
            try
            {
                if (_trainingExercise != null)
                {
                    _trainingExercise.ExerciseUnitType = _trainingExercise.ExerciseUnitType == TExerciseUnitType.RepetitionNumber ? TExerciseUnitType.Time : TExerciseUnitType.RepetitionNumber;
                    await SynchronizeDataAsync();
                }
            }
            catch (Exception except)
            {
                ILogger.Instance.Error("Unable to change exercise unit", except);
                await _userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
        }

        #region Binding Properties

        private ObservableCollection<BindingTrainingExerciseSetRep> _bindingTrainingExerciseSetReps;
        public ObservableCollection<BindingTrainingExerciseSetRep> BindingTrainingExerciseSetReps
        {
            get { return _bindingTrainingExerciseSetReps; }
            set
            {
                _bindingTrainingExerciseSetReps = value;
                OnPropertyChanged();
            }
        }
        
        private string _exerciseTitleLabel;
        public string ExerciseTitleLabel
        {
            get { return _exerciseTitleLabel; }
            set
            {
                _exerciseTitleLabel = value;
                OnPropertyChanged();
            }
        }

        private string _exerciseTitle;
        public string ExerciseTitle
        {
            get { return _exerciseTitle; }
            set
            {
                _exerciseTitle = value;
                OnPropertyChanged();
            }
        }

        private string _restTimeLabel;
        public string RestTimeLabel
        {
            get { return _restTimeLabel; }
            set
            {
                _restTimeLabel = value;
                OnPropertyChanged();
            }
        }

        private int _restTime;
        public int RestTime
        {
            get { return _restTime; }
            set
            {
                _restTime = value;
                OnPropertyChanged();
            }
        }

        private string _eccentricContractionTempoLabel;
        public string EccentricContractionTempoLabel
        {
            get { return _eccentricContractionTempoLabel; }
            set
            {
                _eccentricContractionTempoLabel = value;
                OnPropertyChanged();
            }
        }

        private int _eccentricContractionTempo;
        public int EccentricContractionTempo
        {
            get { return _eccentricContractionTempo; }
            set
            {
                _eccentricContractionTempo = value;
                OnPropertyChanged();
            }
        }

        private string _stretchPositionTempoLabel;
        public string StretchPositionTempoLabel
        {
            get { return _stretchPositionTempoLabel; }
            set
            {
                _stretchPositionTempoLabel = value;
                OnPropertyChanged();
            }
        }

        private int _stretchPositionTempo;
        public int StretchPositionTempo
        {
            get { return _stretchPositionTempo; }
            set
            {
                _stretchPositionTempo = value;
                OnPropertyChanged();
            }
        }

        private string _concentricContractionTempoLabel;
        public string ConcentricContractionTempoLabel
        {
            get { return _concentricContractionTempoLabel; }
            set
            {
                _concentricContractionTempoLabel = value;
                OnPropertyChanged();
            }
        }

        private int _concentricContractionTempo;
        public int ConcentricContractionTempo
        {
            get { return _concentricContractionTempo; }
            set
            {
                _concentricContractionTempo = value;
                OnPropertyChanged();
            }
        }

        private string _contractedPositionTempoLabel;
        public string ContractedPositionTempoLabel
        {
            get { return _contractedPositionTempoLabel; }
            set
            {
                _contractedPositionTempoLabel = value;
                OnPropertyChanged();
            }
        }

        private int _contractedPositionTempo;
        public int ContractedPositionTempo
        {
            get { return _contractedPositionTempo; }
            set
            {
                _contractedPositionTempo = value;
                OnPropertyChanged();
            }
        }

        private string _exerciseImageSource;
        public string ExerciseImageSource
        {
            get { return _exerciseImageSource; }
            set
            {
                _exerciseImageSource = value;
                OnPropertyChanged();
            }
        }

        private string _addRepsLabel;
        public string AddRepsLabel
        {
            get { return _addRepsLabel; }
            set
            {
                _addRepsLabel = value;
                OnPropertyChanged();
            }
        }

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

        private string _unitLabel;
        public string UnitLabel
        {
            get { return _unitLabel; }
            set
            {
                _unitLabel = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Command

        private ICommand _addRepCommand = null;
        public ICommand AddRepCommand
        {
            get
            {
                if (_addRepCommand == null)
                {
                    _addRepCommand = new ViewModelCommand(this, () =>
                    {
                        AddRepAction();
                    });
                }
                return _addRepCommand;
            }
        }

        private ICommand _deleteRepCommand = null;
        public ICommand DeleteRepCommand
        {
            get
            {
                if (_deleteRepCommand == null)
                {
                    _deleteRepCommand = new ViewModelCommand(this, (bindingTrainingExerciseSetRep) =>
                    {
                        DeleteRepAction(bindingTrainingExerciseSetRep as BindingTrainingExerciseSetRep);
                    });
                }
                return _deleteRepCommand;
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

        private ICommand _changeExerciseUnitCommand = null;
        public ICommand ChangeExerciseUnitCommand
        {
            get
            {
                if (_changeExerciseUnitCommand == null)
                {
                    _changeExerciseUnitCommand = new ViewModelCommandAsync(this, async () =>
                    {
                        await ChangeExerciseUnitCommandAction();
                    });
                }
                return _changeExerciseUnitCommand;
            }
        }

        public ApplicationDbContext DbContext { get => _dbContext; set => _dbContext = value; }

        #endregion
    }
}
