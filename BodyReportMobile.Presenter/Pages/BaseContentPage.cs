using System;
using Xamarin.Forms;
using BodyReportMobile.Core.ViewModels;
using Message;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.MvxMessages;

namespace BodyReportMobile.Presenter.Pages
{
	public class BaseContentPage : ContentPage
	{
		public bool DisableBackButton { get; set;} = false;
		public string BackButtonTitle { get; set;} = Translation.Get(TRS.RETURN);

		public BaseContentPage ()
		{
        }
        
        public BaseContentPage(MainViewModel viewModel)
        {
            BindingContext = viewModel;
        }

		public virtual bool CanBackButtonPressing()
		{
			return true;
		}

		protected override bool OnBackButtonPressed()
		{
			if (CanBackButtonPressing ()) {
				this.Navigation.PopAsync ();

				if (this.BindingContext != null && this.BindingContext is BaseViewModel) {
					var baseViewModel = this.BindingContext as BaseViewModel;

                    AppMessenger.AppInstance.Send(new MvxMessageFormClosed(baseViewModel.ViewModelGuid, true));
				}
			}
			return true;
		}
	}
}

