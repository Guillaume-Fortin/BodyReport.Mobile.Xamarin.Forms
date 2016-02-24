using System;
using MvvmCross.Plugins.Messenger;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;

namespace BodyReportMobile.Core.ViewModels
{
	public class MainViewModel  : BaseViewModel
	{
		public MainViewModel (IMvxMessenger messenger) : base(messenger)
		{
		}

		public ICommand GoToTrainingJournalCommand
		{
			get
			{
				return new MvxAsyncCommand (async () => {
					await ShowModalViewModel<TrainingJournalViewModel>(this);
				});
			}
		}

	}
}


