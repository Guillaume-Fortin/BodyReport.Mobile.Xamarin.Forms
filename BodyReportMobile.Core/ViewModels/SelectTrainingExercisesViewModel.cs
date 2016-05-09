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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
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

        public SelectTrainingExercisesViewModel() : base()
        {
            ShowDelayInMs = 0;
            _dbContext = Resolver.Resolve<ISQLite>().GetConnection();
            _bodyExerciseManager = new BodyExerciseManager(_dbContext);
            _muscularGroupManager = new MuscularGroupManager(_dbContext);
            _muscleManager = new MuscleManager(_dbContext);
            _userDialog = Resolver.Resolve<IUserDialogs>();
        }

        protected override async void Show()
        {
            base.Show();

            await SynchronizeData(false, true);
        }

        public static async Task<SelectTrainingExercisesViewModelResut> Show(BaseViewModel parent = null)
        {
            var selectTrainingExercisesViewModelResut = new SelectTrainingExercisesViewModelResut();
            var viewModel = new SelectTrainingExercisesViewModel();
            if(await ShowModalViewModel(viewModel, parent))
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

        private async Task SynchronizeData(bool byPassCurrentAction=false, bool onShow = false)
        {
            try
            {
                if(!byPassCurrentAction)
                    ActionIsInProgress = true;

                if(_muscularGroups == null)
                    _muscularGroups = _muscularGroupManager.FindMuscularGroups();

                if(_muscles == null)
                    _muscles = _muscleManager.FindMuscles();

                if (_bodyExercises == null)
                    _bodyExercises = _bodyExerciseManager.FindBodyExercises();

                if(onShow && _muscularGroups != null && _muscularGroups.Count > 0 && _muscles != null)
                {
                    MuscularGroup = _muscularGroups[0];
                    Muscle = _muscles.Where(m => m.MuscularGroupId == MuscularGroup.Id).FirstOrDefault();
                }

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
                            Name = bodyexercise.Name,
                            Image = "http://192.168.0.15:5000/images/bodyexercises/" + bodyexercise.ImageName,
                        };
                        BindingBodyExercises.Add(bindingBodyExercise);
                    }
                }
                OnPropertyChanged("BindingBodyExercises");
            }
            catch (Exception except)
            {
                await _userDialog.AlertAsync(except.Message, Translation.Get(TRS.ERROR), Translation.Get(TRS.OK));
            }
            finally
            {
                if (!byPassCurrentAction)
                    ActionIsInProgress = false;
            }
        }

        public ICommand SelectMuscularGroupCommand
        {
            get
            {
                return new Command(async () => {
                    if (ActionIsInProgress)
                        return;

                    try
                    {
                        ActionIsInProgress = true;
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

                            var result = await ListViewModel.ShowGenericList(Translation.Get(TRS.MUSCULAR_GROUP), datas, currentData, this);

                            if (result.Validated && result.SelectedData != null)
                            {
                                MuscularGroup = result.SelectedData.Tag as MuscularGroup;
                                Muscle = null;
                                await SynchronizeData(true);
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

        public ICommand SelectMuscleCommand
        {
            get
            {
                return new Command(async () => {
                    if (ActionIsInProgress)
                        return;

                    try
                    {
                        ActionIsInProgress = true;

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

                                var result = await ListViewModel.ShowGenericList(Translation.Get(TRS.MUSCLE), datas, currentData, this);

                                if (result.Validated && result.SelectedData != null)
                                {
                                    Muscle = result.SelectedData.Tag as Muscle;
                                    await SynchronizeData(true);
                                }
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

        public ICommand SelectBodyExerciseCommand
        {
            get
            {
                return new Command((BindingBodyExerciseObject) => {

                    try
                    {
                        var BindingBodyExercise = BindingBodyExerciseObject as BindingBodyExercise;
                        if (BindingBodyExercise != null)
                            BindingBodyExercise.Selected = !BindingBodyExercise.Selected;
                    }
                    catch (Exception exception)
                    {
                    }
                    finally
                    {
                    }

                });
            }
        }

        public ICommand ValidateCommand
        {
            get
            {
                return new Command(async () => {
                    if (ActionIsInProgress)
                        return;

                    try
                    {
                        ActionIsInProgress = true;
                        
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
                    finally
                    {
                        ActionIsInProgress = false;
                    }

                });
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
    }
}
