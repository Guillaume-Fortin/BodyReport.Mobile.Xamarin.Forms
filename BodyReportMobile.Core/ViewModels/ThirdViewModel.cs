using System;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using System.Windows.Input;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.ViewModels
{
	public class ThirdViewModel: BaseViewModel
	{
		public ThirdViewModel (IMvxMessenger messenger) : base(messenger)
		{
		}



		public override void Init(string viewModelGuid)
		{
			base.Init (viewModelGuid);
		}
	}
}

