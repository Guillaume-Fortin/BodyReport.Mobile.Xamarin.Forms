using System;
using MvvmCross.iOS.Views.Presenters;
using UIKit;
using Xamarin.Forms;
using MvvmCross.Platform;
using MvvmCross.Core.ViewModels;
using System.Threading.Tasks;
using MvvmCross.Forms.Presenter.Core;

namespace BodyReport.iOS
{
	public class MvxFormsIosPagePresenter : CustomMvxFormsPagePresenter, IMvxIosViewPresenter
	{
		private readonly UIWindow _window;

		public MvxFormsIosPagePresenter(UIWindow window, Xamarin.Forms.Application mvxFormsApp)
			: base(mvxFormsApp)
		{
			_window = window;
		}

		public virtual bool PresentModalViewController(UIViewController controller, bool animated)
		{
			return false;
		}

		public virtual void NativeModalViewControllerDisappearedOnItsOwn()
		{
		}

		protected override void CustomPlatformInitialization(NavigationPage mainPage)
		{
			_window.RootViewController = mainPage.CreateViewController();
		}
	}
}

