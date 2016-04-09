using System;
using MvvmCross.Core.ViewModels;
using System.Windows.Input;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.ViewModels
{
	public class ThirdViewModel: BaseViewModel
	{
		public ThirdViewModel() : base()
        {
		}

		public override void Init(string viewModelGuid, bool autoClearViewModelDataCollection)
		{
			base.Init (viewModelGuid, autoClearViewModelDataCollection);
		}
	}
}

