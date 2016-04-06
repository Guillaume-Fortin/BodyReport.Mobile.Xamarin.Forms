using System;
using Xamarin.Forms;
using BodyReportMobile.Core;
using MvvmCross.Plugins.Messenger;
using BodyReportMobile.Core.ViewModels;
using Message;
using XLabs.Ioc;

namespace BodyReport
{
	public class BaseContentPage : ContentPage
	{
		public bool DisableBackButton { get; set;} = false;
		public string BackButtonTitle { get; set;} = Translation.Get(TRS.RETURN);

		public BaseContentPage ()
		{
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

					var messenger = Resolver.Resolve<IMvxMessenger>();
					messenger.Publish (new MvxMessageFormClosed (this, baseViewModel.ViewModelGuid, true));
				}
			}
			return true;
		}
	}
}

