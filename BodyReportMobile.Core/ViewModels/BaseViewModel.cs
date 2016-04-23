using System;
using System.Threading.Tasks;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.MvxMessages;
using XLabs.Ioc;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BodyReportMobile.Core.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly string TCS_VALUE = "TCS_VALUE";

        protected bool _autoClearViewModelDataCollection = true;

        /// <summary>
        /// The view model GUID.
        /// </summary>
        private string _viewModelGuid;

        /// <summary>
        /// Title of viewmodel
        /// </summary>
        private string _titleLabel = string.Empty;

        /// <summary>
        /// Data is refreshing in list
        /// </summary>
        private bool _dataIsRefreshing = false;

        /// <summary>
        /// Action in progree
        /// </summary>
        private bool _actionIsInProgress = false;

        public BaseViewModel()
        {
            AppMessenger.AppInstance.Register<MvxMessageFormClosed>(this, OnFormClosedMvxMessage);
            AppMessenger.AppInstance.Register<MvxMessageViewModelEvent>(this, OnViewModelEvent);
        }

        #region view model life cycle

        private async void OnViewModelEvent(MvxMessageViewModelEvent message)
        {
            if (message != null && !string.IsNullOrWhiteSpace(message.ViewModelGuid) &&
                message.ViewModelGuid == _viewModelGuid)
            {
                if (message.Show)
                    Show();
                else if (message.Appear)
                    Appear();
                else if (message.Disappear)
                    Disappear();
                else if (message.Closing)
                    await InternalClosing(message.BackPressed, message.ForceClose, message.ClosingTask);
                else if (message.Closed)
                    Closed();
            }
        }

        /// <summary>
        /// view linked to viewmodel has show
        /// </summary>
        protected virtual void Show()
        {
            InitTranslation();
        }

        /// <summary>
        /// view appear
        /// </summary>
        protected virtual void Appear()
        {
        }

        /// <summary>
        /// view disappear
        /// </summary>
        protected virtual void Disappear()
        {
        }

        /// <returns></returns>
        /// <summary>
        /// For block cloing view linked to viewmodel
        /// Override for display message or block closing view
        /// </summary>
        /// <param name="backPressed">back button pressed</param>
        /// <returns>True if you ahtorize close view</returns>
        protected virtual async Task<bool> Closing(bool backPressed)
        {
            return await Task.FromResult<bool>(!BlockUIAction);
        }

        /// <summary>
        /// For block cloing view linked to viewmodel
        /// <param name="backPressed">back button pressed</param>
        /// <param name="forceClose">force close view (bypass v</param>
        /// </summary>
        protected async Task InternalClosing(bool backPressed, bool forceClose, TaskCompletionSource<bool> ClosingTask)
        {
            if (!forceClose)
            {
                ClosingTask.SetResult(await Closing(backPressed));
            }
            else
                ClosingTask.SetResult(true);
        }

        /// <summary>
        /// Call when view linked to viewmodel closed
        /// Here fo Unregister event message
        /// </summary>
        protected virtual void Closed()
        {
            AppMessenger.AppInstance.Unregister<MvxMessageViewModelEvent>(this);
            AppMessenger.AppInstance.Unregister<MvxMessageFormClosed>(this);
        }

        #endregion

        protected virtual void InitTranslation()
        {
        }

        private void OnFormClosedMvxMessage(MvxMessageFormClosed mvxMessageFormClosed)
        {
            if (mvxMessageFormClosed != null && !string.IsNullOrWhiteSpace(mvxMessageFormClosed.ViewModelGuid) &&
                mvxMessageFormClosed.ViewModelGuid == _viewModelGuid)
            {
                //It's for this view Model
                var tcsShowingViewModel = ViewModelDataCollection.Get<TaskCompletionSource<bool>>(_viewModelGuid, TCS_VALUE);
                if (tcsShowingViewModel != null)
                    tcsShowingViewModel.TrySetResult(!mvxMessageFormClosed.CanceledView);

                if (_autoClearViewModelDataCollection)
                    ViewModelDataCollection.Clear(_viewModelGuid);
            }
        }

        public static async Task<bool> ShowModalViewModel(BaseViewModel viewModel, BaseViewModel parentViewModel, bool mainViewModel = false)
        {
            if (string.IsNullOrWhiteSpace(viewModel.ViewModelGuid))
                viewModel.ViewModelGuid = Guid.NewGuid().ToString();

            var tcs = new TaskCompletionSource<bool>();
            if (!mainViewModel)
                ViewModelDataCollection.Push(viewModel.ViewModelGuid, TCS_VALUE, tcs);

            bool result = false;
            var presenter = Resolver.Resolve<IPresenterManager>();
            if (presenter != null)
            {
                result = await presenter.TryDisplayViewAsync(viewModel, parentViewModel);
            }

            if (mainViewModel && result)
                tcs.SetResult(true);
            else if (!result) //Not awaiting because view is not display
                tcs.SetResult(false);

            return await tcs.Task;
        }

        /// <summary>
        /// Programm demand close view model
        /// </summary>
        /// <returns></returns>
        protected void CloseViewModel(bool cancelView = false)
        {
            AppMessenger.AppInstance.Send(new MvxMessagePageEvent(_viewModelGuid) { ClosingRequest = true, ClosingRequest_ViewCanceled = cancelView });
        }

        #region accessor

        public string ViewModelGuid
        {
            get
            {
                return _viewModelGuid;
            }
            set
            {
                _viewModelGuid = value;
            }
        }

        public string TitleLabel
        {
            get
            {
                return _titleLabel;
            }
            set
            {
                _titleLabel = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region binding

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            // PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        public bool DataIsRefreshing
        {
            get { return _dataIsRefreshing; }
            set
            {
                if (_dataIsRefreshing == value)
                    return;

                _dataIsRefreshing = value;
                OnPropertyChanged();
            }
        }

        public bool ActionIsInProgress
        {
            get { return _actionIsInProgress; }
            set
            {
                if (_actionIsInProgress == value)
                    return;

                _actionIsInProgress = value;
                OnPropertyChanged();
            }
        }

        public bool BlockUIAction
        {
            get { return _dataIsRefreshing || _actionIsInProgress; }
        }
    }
}

