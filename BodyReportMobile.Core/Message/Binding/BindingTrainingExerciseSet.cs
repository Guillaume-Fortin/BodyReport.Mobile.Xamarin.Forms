using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Message.Binding
{
    public class BindingTrainingExerciseSet : NotifyPropertyChanged
    {
        private int _numberOfSets;
        public int NumberOfSets
        {
            get { return _numberOfSets; }
            set
            {
                _numberOfSets = value;
                OnPropertyChanged();
            }
        }

        private int _numberOfReps;
        public int NumberOfReps
        {
            get { return _numberOfReps; }
            set
            {
                _numberOfReps = value;
                OnPropertyChanged();
            }
        }

        private double _weight;
        public double Weight
        {
            get { return _weight; }
            set
            {
                _weight = value;
                OnPropertyChanged();
            }
        }
    }
}
