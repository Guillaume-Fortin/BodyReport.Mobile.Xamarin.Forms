using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Message.Binding
{
    public class BindingTrainingExercise : NotifyPropertyChanged
    {
        public TrainingExercise TrainingExercise { get; set; }
        public int BodyExerciseId { get; set; }

        private string _image;
        public string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }

        private string _bodyExerciseName;
        public string BodyExerciseName
        {
            get { return _bodyExerciseName; }
            set
            {
                _bodyExerciseName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Rest time (second)
        /// </summary>
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

        private string _setReps;
        public string SetReps
        {
            get { return _setReps; }
            set
            {
                _setReps = value;
                OnPropertyChanged();
            }
        }

        private string _setRepsTitle;
        public string SetRepsTitle
        {
            get { return _setRepsTitle; }
            set
            {
                _setRepsTitle = value;
                OnPropertyChanged();
            }
        }

        private string _setRepWeights;
        public string SetRepWeights
        {
            get { return _setRepWeights; }
            set
            {
                _setRepWeights = value;
                OnPropertyChanged();
            }
        }

        private string _setRepWeightsTitle;
        public string SetRepWeightsTitle
        {
            get { return _setRepWeightsTitle; }
            set
            {
                _setRepWeightsTitle = value;
                OnPropertyChanged();
            }
        }

        //public ObservableCollection<BindingTrainingExerciseSet> TrainingExerciseSets { get; set; } = new ObservableCollection<BindingTrainingExerciseSet>();
    }
}
