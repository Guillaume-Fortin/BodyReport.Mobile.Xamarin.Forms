using System;
using MvvmCross.Plugins.Messenger;

namespace BodyReportMobile.Core
{
	public class MvxMessageFormClosed : MvxMessage
	{
		public MvxMessageFormClosed(object sender, string viewModelGuid, bool canceledView) 
			: base(sender)
		{
			ViewModelGuid = viewModelGuid;
			CanceledView = canceledView;
		}

		public string ViewModelGuid { get; private set; }
		public bool CanceledView { get; private set; }
	}
}

