using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MvvmCross.Platform;
using MvvmCross.Core.Views;
using MvvmCross.Core.ViewModels;
using MvvmCross.Forms.Presenter.Droid;
using MvvmCross.Forms.Presenter.Core;

namespace BodyReport.Droid
{
	[Activity (Label = "BodyReport.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);


            var mvxFormsApp = new MvxFormsApp();
            LoadApplication(mvxFormsApp);


            var setup = new Setup(this);
            setup.Initialize();

            var presenter = Mvx.Resolve<IMvxViewPresenter>() as MvxFormsDroidPagePresenter;
            presenter.MvxFormsApp = mvxFormsApp;


            Mvx.Resolve<IMvxAppStart>().Start();
            //LoadApplication (new App ());
		}
	}
}

