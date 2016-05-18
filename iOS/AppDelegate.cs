using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using XLabs.Ioc;
using Acr.UserDialogs;
using BodyReportMobile.Core.Framework;
using BodyReport.iOS.Framework.Renderers;
using BodyReport.iOS.Framework;

namespace BodyReport.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
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
                

			LoadApplication (new BodyReportMobile.Presenter.App ());
			ImageCircleRenderer.Init ();

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
			resolverContainer.Register<ILogger, Logger>();
            resolverContainer.Register<ISQLite, SQLite_iOS>();
            resolverContainer.Register<IMedia, Media>();
            resolverContainer.Register(UserDialogs.Instance);
        }
    }
}

