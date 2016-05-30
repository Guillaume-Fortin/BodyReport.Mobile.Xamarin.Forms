using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Framework.Binding
{
    public class BindingWeekTrainingDay : NotifyPropertyChanged
    {
        /// <summary>
        /// Label
        /// </summary>
		private DayOfWeek _dayOfWeek;
        public DayOfWeek DayOfWeek
        {
            get { return _dayOfWeek; }
            set
            {
                _dayOfWeek = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Label
        /// </summary>
		private string _label;
        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// TrainingDayExist
        /// </summary>
		private bool _trainingDayExist;
        public bool TrainingDayExist
        {
            get { return _trainingDayExist; }
            set
            {
                _trainingDayExist = value;
                OnPropertyChanged();
            }
        }
    }
}
