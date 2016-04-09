using System;
using MvvmCross.Core.ViewModels;
using System.Windows.Input;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.ViewModels
{
	public class SecondViewModel: BaseViewModel
	{
		public SecondViewModel() : base()
        {
		}

		public ICommand DisplayViewCommand
		{
			get
			{
				return new MvxCommand(Test);
			}
		}

		void Test()
		{
			//await ShowModalViewModel<ThirdViewModel>(this);
		}
	}
}


