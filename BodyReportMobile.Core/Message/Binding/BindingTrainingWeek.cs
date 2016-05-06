using System;
using Message;

namespace BodyReportMobile.Core.Message.Binding
{
	public class BindingTrainingWeek : NotifyPropertyChanged
	{
		public string Year {get; set;}
		public string Week { get; set;}
        public string Date { get; set; }
		public TrainingWeek TrainingWeek {get; set;}
	}
}

