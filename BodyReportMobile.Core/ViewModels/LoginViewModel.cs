using System;
using BodyReportMobile.Core.ViewModels;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using Message;
using BodyReportMobile.Core.Framework;

namespace BodyReportMobile.Core.ViewModels
{
	public class LoginViewModel : BaseViewModel
	{
		public string LoginLabel {get; set;}

		public LoginViewModel () : base()
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
				return new MvxCommand (() => {
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

