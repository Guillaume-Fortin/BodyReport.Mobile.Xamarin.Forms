using System;
using System.Threading.Tasks;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.MvxMessages;
using XLabs.Ioc;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Message;
using System.Windows.Input;

namespace BodyReportMobile.Core.ViewModels
{
    public class BaseViewModel : NotifyPropertyChanged
    {
        private static readonly string TCS_VALUE = "TCS_VALUE";

        protected bool _autoClearViewModelDataCollection = true;

        protected bool _allowCancelViewModel = true;

        protected bool _viewModelClosing = false;
        protected bool _viewModelClosed = false;

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
        /// Action in progress
        /// </summary>
        private bool _actionIsInProgress = false;

        /// <summary>
        /// Show view model with delay
        /// </summary>
        private int _showDelayInMs = 200;

        public BaseViewModel()
        {
            AppMessenger.AppInstance.Register<MvxMessageViewModelEvent>(this, OnViewModelEventAsync);
            ActionIsInProgress = true;
        }

        #region view model life cycle

        private async void OnViewModelEventAsync(MvxMessageViewModelEvent message)
        {
            if (message != null && !string.IsNullOrWhiteSpace(message.ViewModelGuid) &&
                message.ViewModelGuid == _viewModelGuid)
            {
                if (message.Show)
                    await InternalShowAsync();
                else if (message.Appear)
                    Appear();
                else if (message.Disappear)
                    Disappear();
                else if (message.Closing)
                    await InternalClosingAsync(message.BackPressed, message.ForceClose, message.ClosingTask);
                else if (message.Closed)
                    Closed(message.BackPressed);
            }
        }

        /// <summary>
        /// view linked to viewmodel has show
        /// </summary>
        protected virtual Task ShowAsync()
        {
            InitTranslation();
            return Task.FromResult(false);
        }

        private async Task InternalShowAsync()
        {
            ActionIsInProgress = false;
            await ShowAsync();
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
        protected virtual async Task<bool> ClosingAsync(bool backPressed)
        {
            if (!backPressed || (backPressed && _allowCancelViewModel))
                return await Task.FromResult<bool>(!BlockUIAction);
            else
                return await Task.FromResult<bool>(false);
        }

        /// <summary>
        /// For block cloing view linked to viewmodel
        /// <param name="backPressed">back button pressed</param>
        /// <param name="forceClose">force close view (bypass v</param>
        /// </summary>
        protected async Task InternalClosingAsync(bool backPressed, bool forceClose, TaskCompletionSource<bool> ClosingTask)
        {
            if (!forceClose)
            {
                bool closeAuthorized = await ClosingAsync(backPressed);
                if (!closeAuthorized)
                    _viewModelClosing = false;
                ClosingTask.SetResult(closeAuthorized);
            }
            else
                ClosingTask.SetResult(true);
        }

        /// <summary>
        /// Call when view linked to viewmodel closed
        /// Here fo Unregister event message
        /// </summary>
        protected virtual void Closed(bool backPressed)
        {
            _viewModelClosed = true;

            //It's for this view Model
            var tcsShowingViewModel = ViewModelDataCollection.Get<TaskCompletionSource<bool>>(_viewModelGuid, TCS_VALUE);
            if (tcsShowingViewModel != null)
                tcsShowingViewModel.TrySetResult(!backPressed);

            if (_autoClearViewModelDataCollection)
                ViewModelDataCollection.Clear(_viewModelGuid);

            AppMessenger.AppInstance.Unregister<MvxMessageViewModelEvent>(this);
        }

        #endregion

        protected virtual void InitTranslation()
        {
        }
        
        protected static async Task<bool> ShowModalViewModelAsync(BaseViewModel viewModel, BaseViewModel parentViewModel, bool mainViewModel = false, bool hideTitleBar = false)
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
                result = await presenter.TryDisplayViewAsync(viewModel, parentViewModel, hideTitleBar);
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
            if (_viewModelClosing || _viewModelClosed) // Security
                return;
            DataIsRefreshing = false;
            ActionIsInProgress = false;
            _viewModelClosing = true;
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
                _actionIsInProgress = value;
                OnPropertyChanged();
            }
        }

        public bool BlockUIAction
        {
            get { return _dataIsRefreshing || _actionIsInProgress; }
        }

        public int ShowDelayInMs
        {
            get
            {
                return _showDelayInMs;
            }

            set
            {
                _showDelayInMs = value;
            }
        }

        #endregion
    }
}

