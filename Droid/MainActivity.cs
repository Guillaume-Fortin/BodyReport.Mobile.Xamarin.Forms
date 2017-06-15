using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using XLabs.Ioc;
using Acr.UserDialogs;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Presenter;
using Android.Graphics.Drawables;
using BodyReport.Droid.Framework;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using V7Toolbar = Android.Support.V7.Widget.Toolbar;

namespace BodyReport.Droid
{
	[Activity (MainLauncher = false, //Indicates the theme to use for this activity
	           Label = "BodyReport", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : FormsAppCompatActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			// this line is essential to wiring up the toolbar styles defined in ~/Resources/layout/toolbar.axml
			FormsAppCompatActivity.ToolbarResource = Resource.Layout.toolbar;

			base.OnCreate (savedInstanceState);
            
            if(!Resolver.IsSet)
            {
                var resolverContainer = new SimpleContainer();
                resolverContainer.Register<IDependencyContainer>(resolverContainer);
                Resolver.SetResolver(resolverContainer.GetResolver());

                UserDialogs.Init(this);
                AddIocDependencies();
            }
            
			global::Xamarin.Forms.Forms.Init (this, savedInstanceState);
			LoadApplication (new App ());
            //ActionBar.SetIcon(new ColorDrawable(Android.Graphics.Color.Transparent));

            V7Toolbar toolbar = this.FindViewById<V7Toolbar>(Resource.Id.testToolbar);
            if (toolbar != null)
                SetSupportActionBar(toolbar);
        }

        /// <summary>
		/// Adds the ioc dependencies.
		/// </summary>
		private void AddIocDependencies()
        {
            var resolverContainer = Resolver.Resolve<IDependencyContainer>();
            resolverContainer.Register<ISecurity, SecurityDroid>();
            resolverContainer.Register<IAndroidAPI, AndroidAPI>();
            resolverContainer.Register<IFileManager, FileManager>();
            resolverContainer.Register<ISQLite, SQLite_Droid>();
            resolverContainer.Register<ILogger, Logger>();
            resolverContainer.Register<IMedia, Media>();
            resolverContainer.Register(UserDialogs.Instance);
        }

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			return base.OnCreateOptionsMenu (menu);
		}

		public override bool OnSupportNavigateUp ()
		{
			var parent = this.SupportParentActivityIntent;
			base.OnBackPressed ();
			return false;
		}

		public override void OnBackPressed ()
		{
			base.OnBackPressed ();
		}

		/// <summary>
		/// Action bar menu item selected
		/// Used for redirect back button press on hardware back button pressed
		/// </summary>
		/// <param name="item">menu item</param>
		/// <returns>True or False</returns>
		public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.home:
                    OnBackPressed();
                    break;
                case Resource.Id.homeAsUp:
                    OnBackPressed();
                    break;
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    break;
                default:
                    return base.OnOptionsItemSelected(item);
            }
            return false;
        }
    }
}

