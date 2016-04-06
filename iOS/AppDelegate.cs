using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using MvvmCross.iOS.Platform;
using MvvmCross.iOS.Views.Presenters;
using MvvmCross.Platform;
using MvvmCross.Core.ViewModels;
using XLabs.Ioc;
using BodyReportMobile.Core;
using Acr.UserDialogs;

namespace BodyReport.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : MvxApplicationDelegate
	{
		UIWindow _window;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
            global::Xamarin.Forms.Forms.Init();

            if (!Resolver.IsSet)
                AddIocDependencies();

            _window = new UIWindow(UIScreen.MainScreen.Bounds);

            var setup = new Setup(this, _window);
			setup.Initialize();

			var startup = Resolver.Resolve<IMvxAppStart>();
			startup.Start();

			_window.MakeKeyAndVisible();

			return true;

            /*
			_window = new UIWindow(UIScreen.MainScreen.Bounds);
			_window.BackgroundColor = UIColor.White;

			var setup = new Setup(this, _window);
			setup.Initialize();

			var startup = Resolver.Resolve<IMvxAppStart>();
			startup.Start();

			_window.MakeKeyAndVisible();

			return true;*/
        }

        /// <summary>
		/// Adds the ioc dependencies.
		/// </summary>
		private void AddIocDependencies()
        {
            var resolverContainer = new SimpleContainer();

            resolverContainer.Register<ISecurity, SecurityIOS>();
            resolverContainer.Register<IFileManager, FileManager>();
            resolverContainer.Register<ISQLite, SQLite_iOS>();
            resolverContainer.Register(UserDialogs.Instance);

            Resolver.SetResolver(resolverContainer.GetResolver());
        }
    }
}

