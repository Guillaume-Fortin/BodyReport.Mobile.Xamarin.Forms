using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MvvmCross.Core.Views;
using MvvmCross.Core.ViewModels;
using MvvmCross.Forms.Presenter.Droid;
using MvvmCross.Forms.Presenter.Core;
using XLabs.Ioc;
using BodyReportMobile.Core;
using Acr.UserDialogs;
using BodyReportMobile.Core.ViewModels;
using BodyReportMobile.Core.Framework;

namespace BodyReport.Droid
{
	[Activity (Label = "BodyReport.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            if(!Resolver.IsSet)
            {
                var resolverContainer = new SimpleContainer();
                resolverContainer.Register<IDependencyContainer>(resolverContainer);
                Resolver.SetResolver(resolverContainer.GetResolver());

                AddIocDependencies();
            }

            var mvxFormsApp = new MvxFormsApp();
            LoadApplication(mvxFormsApp);


            var setup = new Setup(this);
            setup.Initialize();

            var presenter = Resolver.Resolve<IMvxViewPresenter>() as MvxFormsDroidPagePresenter;
            presenter.MvxFormsApp = mvxFormsApp;
            
            Resolver.Resolve<BaseViewModel>().Start();
            //LoadApplication (new App ());
		}

        /// <summary>
		/// Adds the ioc dependencies.
		/// </summary>
		private void AddIocDependencies()
        {
            var resolverContainer = Resolver.Resolve<IDependencyContainer>();
            resolverContainer = Resolver.Resolve<IDependencyContainer>();
            resolverContainer.Register<ISecurity, SecurityDroid>();
            resolverContainer.Register<IFileManager, FileManager>();
            resolverContainer.Register<ISQLite, SQLite_Droid>();
            resolverContainer.Register(UserDialogs.Instance);

            /*var resolverContainer = new SimpleContainer();

        resolverContainer.Register<ISecurity, SecurityDroid>();
        resolverContainer.Register<IFileManager, FileManager>();
        resolverContainer.Register<ISQLite, SQLite_Droid>();
        resolverContainer.Register(UserDialogs.Instance);

        Resolver.SetResolver(resolverContainer.GetResolver());*/
        }
    }
}

