using System;
using BodyReportMobile.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using Message;

namespace BodyReportMobile.Core
{
	public class LoginViewModel : BaseViewModel
	{
		public string LoginLabel {get; set;}

		public LoginViewModel (IMvxMessenger messenger) : base(messenger)
		{
		}

		protected override void InitTranslation()
		{
			base.InitTranslation ();

			TitleLabel = Translation.Get (TRS.CONNECTION);
			LoginLabel = Translation.Get (TRS.LOG_IN);
		}

		public override void Init(string viewModelGuid, bool autoClearViewModelDataCollection)
		{
			base.Init (viewModelGuid, autoClearViewModelDataCollection);
		}

		public ICommand LoginCommand
		{
			get
			{
				return new MvxAsyncCommand (async () => {
					if(ValidateFields())
					{
						CloseViewModel();
					}
				});
			}
		}

		private bool ValidateFields()
		{
			return false;
		}
	}
}

