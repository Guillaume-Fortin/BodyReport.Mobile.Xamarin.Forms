using System;
using Xamarin.Forms;
using MvvmCross.Forms.Presenter.Core;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System.Threading.Tasks;	
using MvvmCross.Core.Views;
using BodyReportMobile.Core;
using MvvmCross.Plugins.Messenger;

namespace BodyReport
{
	//View https://github.com/MvvmCross/MvvmCross-Forms/blob/4f026c6acdba5b064ae36be83f04bd12dbb00a02/MvvmCross.Forms.Presenter.Core/MvxFormsPagePresenter.cs
	public class CustomMvxFormsPagePresenter : MvxFormsPagePresenter
	{
		public CustomMvxFormsPagePresenter (Application app) : base(app)
		{
		}

		protected override void CustomPlatformInitialization(NavigationPage mainPage)
		{
			base.CustomPlatformInitialization (mainPage);
		}

		public override void Show(MvxViewModelRequest request)
		{
			if (TryShowPage(request))
				return;

			Mvx.Error("Skipping request for {0}", request.ViewModelType.Name);
		}

		private bool TryShowPage(MvxViewModelRequest request)
		{
			var page = MvxPresenterHelpers.CreatePage(request);
			if (page == null)
				return false;

			var viewModel = MvxPresenterHelpers.LoadViewModel(request);

			var mainPage = MvxFormsApp.MainPage as NavigationPage;
			page.BindingContext = viewModel;

			if (mainPage == null)
			{
				MvxFormsApp.MainPage = new NavigationPage(page);
				mainPage = MvxFormsApp.MainPage as NavigationPage;
				CustomPlatformInitialization(mainPage);
			}
			else
			{
				try
				{
					/*if (viewModel is BaseViewModel) {
						page.Disappearing += Page_Disappearing;
					}*/

					// calling this sync blocks UI and never navigates hence code continues regardless here
					mainPage.PushAsync(page);
				}
				catch (Exception e)
				{
					Mvx.Error("Exception pushing {0}: {1}\n{2}", page.GetType(), e.Message, e.StackTrace);
					return false;
				}
			}

			return true;
		}

        /*
		void Page_Disappearing (object sender, EventArgs e)
		{
			if (sender == null || !(sender is Page))
				return;

			var page = sender as Page;

			if (page.BindingContext != null && page.BindingContext is BaseViewModel) {
				var baseViewModel = page.BindingContext as BaseViewModel;

				var messenger = Resolver.Resolve<IMvxMessenger>();
				messenger.Publish (new MvxMessageFormClosed (page, baseViewModel.ViewModelGuid, true));
			}
		}*/
    }
}


