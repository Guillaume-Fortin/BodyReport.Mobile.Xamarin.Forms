using System;

namespace BodyReportMobile.Core.MvxMessages
{
	public class MvxMessageFormClosed
	{
		public MvxMessageFormClosed(string viewModelGuid, bool canceledView) 
		{
			ViewModelGuid = viewModelGuid;
			CanceledView = canceledView;
		}

		public string ViewModelGuid { get; private set; }
		public bool CanceledView { get; private set; }
	}
}

