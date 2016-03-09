using System;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using System.Windows.Input;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.ViewModels
{
	public class SecondViewModel: BaseViewModel
	{
		public SecondViewModel (IMvxMessenger messenger) : base(messenger)
		{
		}

		public ICommand DisplayViewCommand
		{
			get
			{
				return new MvxAsyncCommand(Test);
			}
		}

		async Task Test()
		{
			//await ShowModalViewModel<ThirdViewModel>(this);
		}
	}
}


