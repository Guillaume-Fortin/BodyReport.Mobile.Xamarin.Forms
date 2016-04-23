using System;
using Xamarin.Forms;
using BodyReportMobile.Core.ViewModels;
using Message;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.MvxMessages;
using System.Threading.Tasks;

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
            RegisterEvent();
        }

		public virtual bool CanBackButtonPressing()
		{
			return true;
		}

        private void RegisterEvent()
        {
            AppMessenger.AppInstance.Register<MvxMessagePageEvent>(this, OnPageEvent);
        }

        private void UnRegisterEvent()
        {
            AppMessenger.AppInstance.Unregister<MvxMessagePageEvent>(this);
        }

        private void OnPageEvent(MvxMessagePageEvent message)
        {
            if (_viewModel != null && message != null && !string.IsNullOrWhiteSpace(message.ViewModelGuid) &&
                message.ViewModelGuid == _viewModel.ViewModelGuid)
            {
                if (message.ClosingRequest)
                    CloseView(message.ClosingRequest_ViewCanceled);
            }
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
                CloseView(true);
            }

            // If you want to stop the back button
            return true;
        }

        private void CloseView(bool cancelView)
        {
            try
            {
                UnRegisterEvent();
                this.Navigation.PopAsync();
                if (_viewModel != null)
                    AppMessenger.AppInstance.Send(new MvxMessageFormClosed(_viewModel.ViewModelGuid, cancelView));
            }
            catch
            {
                //TODO LOG
            }
        }
        
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if(_firstViewAppear)
            {
                _firstViewAppear = false;
                if(_viewModel != null)
                {
                    await Task.Delay(200); // Necessary for wait update ui (Ex : activity indicator in listview)
                    AppMessenger.AppInstance.Send(new MvxMessageViewModelEvent(_viewModel.ViewModelGuid) { Show = true });
                }   
            }
            else
            {
                if (_viewModel != null)
                    AppMessenger.AppInstance.Send(new MvxMessageViewModelEvent(_viewModel.ViewModelGuid) { Appear = true });
            }

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (_viewModel != null)
                AppMessenger.AppInstance.Send(new MvxMessageViewModelEvent(_viewModel.ViewModelGuid) { Disappear = true });
        }
    }
}

