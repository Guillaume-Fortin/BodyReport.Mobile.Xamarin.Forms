using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using XLabs.Ioc;
using Acr.UserDialogs;
using BodyReportMobile.Core.Framework;

namespace BodyReport.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
		UIWindow _window;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
            global::Xamarin.Forms.Forms.Init();

            if (!Resolver.IsSet)
            {
                var resolverContainer = new SimpleContainer();
                resolverContainer.Register<IDependencyContainer>(resolverContainer);
                Resolver.SetResolver(resolverContainer.GetResolver());

                AddIocDependencies();
            }
                

            _window = new UIWindow(UIScreen.MainScreen.Bounds);

            return base.FinishedLaunching(app, options);
        }

        /// <summary>
		/// Adds the ioc dependencies.
		/// </summary>
		private void AddIocDependencies()
        {
            var resolverContainer = Resolver.Resolve<IDependencyContainer>();
            resolverContainer.Register<ISecurity, SecurityIOS>();
            resolverContainer.Register<IFileManager, FileManager>();
            resolverContainer.Register<ISQLite, SQLite_iOS>();
            resolverContainer.Register(UserDialogs.Instance);
        }
    }
}

