using System;
using BodyReportMobile.Core;
using XLabs.Ioc;
using BodyReportMobile.Core.ViewModels;
using BodyReportMobile.Presenter.Pages;
using BodyReportMobile.Core.Framework;
using Xamarin.Forms;
using BodyReportMobile.Core.ViewModels.Generic;
using BodyReportMobile.Presenter.Pages.Generics;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace BodyReportMobile.Presenter
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent ();
            Initialize();
		}

		private async void Initialize()
		{
            Translation.LoadTranslation();
            
            var resolverContainer = Resolver.Resolve<IDependencyContainer>();
            RegisterViewModelViewDependencies(resolverContainer);

            bool result = await MainViewModel.ShowAsync(null);

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

            presenterManager.AddViewDependency<DataSyncViewModel, DataSyncPage>();
            presenterManager.AddViewDependency<MainViewModel, MainPage>();
            presenterManager.AddViewDependency<MenuViewModel, MenuPage>();
            presenterManager.AddViewDependency<TrainingJournalViewModel, TrainingJournalPage>();
            presenterManager.AddViewDependency<EditTrainingWeekViewModel, EditTrainingWeekPage>();
            presenterManager.AddViewDependency<TrainingWeekViewModel, TrainingWeekPage>();
            presenterManager.AddViewDependency<LoginViewModel, LoginPage>();
            presenterManager.AddViewDependency<RegisterAccountViewModel, RegisterAccountPage>();
            presenterManager.AddViewDependency<EditUserProfileViewModel, EditUserProfilePage>();
            presenterManager.AddViewDependency<ListViewModel, ListPage>();
            presenterManager.AddViewDependency<CopyTrainingWeekViewModel, CopyTrainingWeekPage>();
            presenterManager.AddViewDependency<CreateTrainingDayViewModel, CreateTrainingDayPage>();
            presenterManager.AddViewDependency<TrainingDayViewModel, TrainingDayPage>();
            presenterManager.AddViewDependency<SelectTrainingExercisesViewModel, SelectTrainingExercisesPage> ();
            presenterManager.AddViewDependency<EditTrainingExerciseViewModel, EditTrainingExercisePage>();

            resolverContainer.Register<IPresenterManager> (presenterManager);
        }

        protected override void OnStart()
        {
            base.OnStart();
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            base.OnResume();
            // Handle when your app resumes
        }

		public static bool GetXamlResources<T>(string resourceName, out T value)
		{
			value = default(T);
			if (App.Current.Resources != null && App.Current.Resources.ContainsKey (resourceName))
			{
				value = (T)App.Current.Resources [resourceName];
				return true;
			}
			return false;
		}

		public static T GetXamlResources<T>(string resourceName)
		{
			if (App.Current.Resources != null && App.Current.Resources.ContainsKey (resourceName))
			{
				return (T)App.Current.Resources [resourceName];
			}
			return default(T);
		}
    }
}

