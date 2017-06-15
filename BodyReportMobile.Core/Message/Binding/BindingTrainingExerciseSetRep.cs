using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Message.Binding
{
    public class BindingTrainingExerciseSetRep : NotifyPropertyChanged
    {
        private string _repsLabel = string.Empty; // necessary for trigger Text.Length
        public string RepsLabel
        {
            get { return _repsLabel; }
            set
            {
                _repsLabel = value;
                OnPropertyChanged();
            }
        }

        private string _weightsLabel = string.Empty; // necessary for trigger Text.Length
        public string WeightsLabel
        {
            get { return _weightsLabel; }
            set
            {
                _weightsLabel = value;
                OnPropertyChanged();
            }
        }

        private int _repsOrExecTimes;
        public int RepsOrExecTimes
        {
            get { return _repsOrExecTimes; }
            set
            {
                if (_repsOrExecTimes != value)
                {
                    _repsOrExecTimes = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _weights;
        public double Weights
        {
            get { return _weights; }
            set
            {
                if (_weights != value)
                {
                    _weights = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _btnPlusVisible;
        public bool BtnPlusVisible
        {
            get { return _btnPlusVisible; }
            set
            {
                _btnPlusVisible = value;
                OnPropertyChanged();
            }
        }
    }
}
