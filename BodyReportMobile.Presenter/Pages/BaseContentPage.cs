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
        private bool _firstViewAppear = true;
        private BaseViewModel _viewModel = null;

        public bool DisableBackButton { get; set;} = false;
		public string BackButtonTitle { get; set;} = Translation.Get(TRS.RETURN);

		public BaseContentPage ()
		{
        }
        
        public BaseContentPage(BaseViewModel viewModel)
        {
            _viewModel = viewModel;
            BindingContext = viewModel;
        }

		public virtual bool CanBackButtonPressing()
		{
			return true;
		}

        /// <summary>
        /// Intercept Back button press by user (only physical and logical hardware button)
        /// Not for back button inside view on iOS and Android
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackButtonPressed()
		{
            base.OnBackButtonPressed();
            if (CanBackButtonPressing ()) {
				this.Navigation.PopAsync ();

				if (_viewModel != null)
                    AppMessenger.AppInstance.Send(new MvxMessageFormClosed(_viewModel.ViewModelGuid, true));
            }

            // If you want to stop the back button
            return true;
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(_firstViewAppear)
            {
                _firstViewAppear = false;
                if(_viewModel != null)
                    AppMessenger.AppInstance.Send(new MvxMessageViewEvent(_viewModel.ViewModelGuid) { Show = true });
            }
            else
            {
                if (_viewModel != null)
                    AppMessenger.AppInstance.Send(new MvxMessageViewEvent(_viewModel.ViewModelGuid) { Appear = true });
            }

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (_viewModel != null)
                AppMessenger.AppInstance.Send(new MvxMessageViewEvent(_viewModel.ViewModelGuid) { Disappear = true });
        }
    }
}

