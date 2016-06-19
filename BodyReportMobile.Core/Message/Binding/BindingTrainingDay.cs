using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Message.Binding
{
    public class BindingTrainingDay : NotifyPropertyChanged
    {
        public string UserId { get; set; }

        private int _year;
        public int Year
        {
            get{ return _year; }
            set
            {
                _year = value;
                OnPropertyChanged();
            }
        }

        private int _weekOfYear;
        public int WeekOfYear
        {
            get { return _weekOfYear; }
            set
            {
                _weekOfYear = value;
                OnPropertyChanged();
            }
        }

        private int _day;
        public int Day
        {
            get { return _day; }
            set
            {
                _day = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan _beginTime;
        public TimeSpan BeginTime
        {
            get { return _beginTime; }
            set
            {
                _beginTime = value;
                ChangeEndTime();
                OnPropertyChanged();
            }
        }

        private TimeSpan _endTime;
        public TimeSpan EndTime
        {
            get { return _endTime; }
            set
            {
                _endTime = value;
                OnPropertyChanged();
            }
        }
        
        private string _dayLabel;
        public string DayLabel
        {
            get { return _dayLabel; }
            set
            {
                _dayLabel = value;
                OnPropertyChanged();
            }
        }

        private string _beginTimeLabel;
        public string BeginTimeLabel
        {
            get { return _beginTimeLabel; }
            set
            {
                _beginTimeLabel = value;
                OnPropertyChanged();
            }
        }

        private string _endTimeLabel;
        public string EndTimeLabel
        {
            get { return _endTimeLabel; }
            set
            {
                _endTimeLabel = value;
                OnPropertyChanged();
            }
        }
        
        private void ChangeEndTime()
        {
            if(EndTime < BeginTime)
            {
                var newEndTime = BeginTime.Add(new TimeSpan(0, 45, 0));
                var maxTime = new TimeSpan(23, 59, 0);
                if (newEndTime > maxTime)
                    newEndTime = maxTime;
                EndTime = newEndTime;
            }
        }
    }
}
