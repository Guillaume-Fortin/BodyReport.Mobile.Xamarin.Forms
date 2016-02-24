using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using MvvmCross.iOS.Platform;
using MvvmCross.iOS.Views.Presenters;
using MvvmCross.Platform;
using MvvmCross.Core.ViewModels;

namespace BodyReport.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : MvxApplicationDelegate
	{
		UIWindow _window;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			_window = new UIWindow(UIScreen.MainScreen.Bounds);

			var setup = new Setup(this, _window);
			setup.Initialize();

			var startup = Mvx.Resolve<IMvxAppStart>();
			startup.Start();

			_window.MakeKeyAndVisible();

			return true;

			/*
			_window = new UIWindow(UIScreen.MainScreen.Bounds);
			_window.BackgroundColor = UIColor.White;

			var setup = new Setup(this, _window);
			setup.Initialize();

			var startup = Mvx.Resolve<IMvxAppStart>();
			startup.Start();

			_window.MakeKeyAndVisible();

			return true;*/
		}
	}
}

