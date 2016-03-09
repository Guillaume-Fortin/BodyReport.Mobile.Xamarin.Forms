using System;
using MvvmCross.Core.ViewModels;
using System.Threading.Tasks;
using MvvmCross.Plugins.Messenger;
using MvvmCross.Platform;

namespace BodyReportMobile.Core.ViewModels
{
	public class BaseViewModel : MvxViewModel
	{
		private static readonly string TCS_VALUE = "TCS_VALUE";

		private bool _autoClearViewModelDataCollection;

		/// <summary>
		/// The view model GUID.
		/// </summary>
		private string _viewModelGuid;

		/// <summary>
		/// The mvx messenger token.
		/// </summary>
		private readonly MvxSubscriptionToken _mvxMessengerFormClosedToken;

		public BaseViewModel (IMvxMessenger messenger)
		{
			_mvxMessengerFormClosedToken = messenger.Subscribe<MvxMessageFormClosed>(OnFormClosedMvxMessage);
			if (_mvxMessengerFormClosedToken == null) // supress unused Warning
				_mvxMessengerFormClosedToken = null;
		}

		public virtual void Init(string viewModelGuid, bool autoClearViewModelDataCollection)
		{
			_viewModelGuid = viewModelGuid;
			_autoClearViewModelDataCollection = autoClearViewModelDataCollection;
			InitTranslation ();
		}

		protected virtual void InitTranslation()
		{
		}

		private void OnFormClosedMvxMessage(MvxMessageFormClosed mvxMessageFormClosed)
		{
			if (mvxMessageFormClosed != null && !string.IsNullOrWhiteSpace (mvxMessageFormClosed.ViewModelGuid) && 
				mvxMessageFormClosed.ViewModelGuid == _viewModelGuid) {
				//It's for this view Model
				var tcsShowingViewModel = ViewModelDataCollection.Get<TaskCompletionSource<bool>> (_viewModelGuid, TCS_VALUE);
				if (tcsShowingViewModel != null)
					tcsShowingViewModel.SetResult (!mvxMessageFormClosed.CanceledView);

				if(_autoClearViewModelDataCollection)
					ViewModelDataCollection.Clear (_viewModelGuid);
			}
		}

		protected static async Task<bool> ShowModalViewModel<TViewModel>(BaseViewModel baseMvxViewModel) where TViewModel : MvxViewModel
		{
			string viewModelGuid = Guid.NewGuid ().ToString ();
			return await ShowModalViewModel<TViewModel>(viewModelGuid, true, baseMvxViewModel);
		}

		protected static async Task<bool> ShowModalViewModel<TViewModel>(string viewModelGuid, bool autoClearViewModelDataCollection, BaseViewModel baseMvxViewModel) where TViewModel : MvxViewModel
		{
			var tcs = new TaskCompletionSource<bool>();
			ViewModelDataCollection.Push (viewModelGuid, TCS_VALUE, tcs);
			
			bool result = baseMvxViewModel.ShowViewModel<TViewModel> (new { viewModelGuid = viewModelGuid, autoClearViewModelDataCollection = autoClearViewModelDataCollection});

			if (!result) //Not awaiting because view is not display
				tcs.SetResult (false);
			
			return await tcs.Task;
		}

		protected bool CloseViewModel()
		{
			if (Close (this)) {
				var messenger = Mvx.Resolve<IMvxMessenger> ();
				messenger.Publish (new MvxMessageFormClosed (this, ViewModelGuid, false));
				return true;
			}
			return false;
		}

		#region accessor

		public string ViewModelGuid {
			get {
				return _viewModelGuid;
			}
		}

		#endregion
	}
}

