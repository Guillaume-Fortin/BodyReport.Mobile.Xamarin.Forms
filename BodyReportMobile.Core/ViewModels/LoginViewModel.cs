using System;
using BodyReportMobile.Core.ViewModels;
using System.Windows.Input;
using Message;
using BodyReportMobile.Core.Framework;
using Xamarin.Forms;

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

		public ICommand LoginCommand
		{
			get
			{
				return new Command (() => {
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

