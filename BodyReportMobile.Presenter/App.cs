using System;
using BodyReportMobile.Core;
using XLabs.Ioc;
using BodyReportMobile.Core.ViewModels;
using BodyReportMobile.Presenter.Pages;
using BodyReportMobile.Core.Framework;
using Xamarin.Forms;

namespace BodyReportMobile.Presenter
{
	public class App : Application
	{
		public App()
		{
            Initialize();
		}

		private async void Initialize()
		{
            Translation.LoadTranslation();
            
            var resolverContainer = Resolver.Resolve<IDependencyContainer>();
            RegisterViewModelViewDependencies(resolverContainer);

            var mainViewModel = new MainViewModel();
            bool result = await BaseViewModel.ShowModalViewModel<MainViewModel>(mainViewModel);

            if(result)
            {
                var presenter = Resolver.Resolve<IPresenterManager>() as PresenterManager;
                if (presenter != null)
                {
                    MainPage = presenter.MainNavigationPage;
                    result = true;
                }
            }
        }

        private void RegisterViewModelViewDependencies(IDependencyContainer resolverContainer)
        {
            var presenterManager = new PresenterManager();

            presenterManager.AddViewDependency<MainViewModel, MainPage>();
            presenterManager.AddViewDependency<TrainingJournalViewModel, TrainingJournalPage>();
            presenterManager.AddViewDependency<EditTrainingWeekViewModel, EditTrainingWeekPage>();
            presenterManager.AddViewDependency<LoginViewModel, LoginPage>();

            resolverContainer.Register<IPresenterManager> (presenterManager);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

