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

namespace BodyReport.Droid
{
	[Activity (MainLauncher = true, Label = "BodyReport", Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
            
            if(!Resolver.IsSet)
            {
                var resolverContainer = new SimpleContainer();
                resolverContainer.Register<IDependencyContainer>(resolverContainer);
                Resolver.SetResolver(resolverContainer.GetResolver());

                AddIocDependencies();
            }
            
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App ());
            ActionBar.SetIcon(new ColorDrawable(Android.Graphics.Color.Transparent));
        }

        /// <summary>
		/// Adds the ioc dependencies.
		/// </summary>
		private void AddIocDependencies()
        {
            var resolverContainer = Resolver.Resolve<IDependencyContainer>();
            resolverContainer.Register<ISecurity, SecurityDroid>();
            resolverContainer.Register<IFileManager, FileManager>();
            resolverContainer.Register<ISQLite, SQLite_Droid>();
            resolverContainer.Register(UserDialogs.Instance);
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

