using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using MvvmCross.Droid.Platform;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.Platform;
using MvvmCross.Droid.Views;
using MvvmCross.Forms.Presenter.Droid;
using MvvmCross.Core.Views;
using XLabs.Ioc;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Presenter;

namespace BodyReport.Droid
{
    public class Setup : MvxAndroidSetup
    {
        public Setup(Context applicationContext)
            : base(applicationContext)
        {
            
        }

        protected override IMvxApplication CreateApp()
        {
            return new App();
        }

        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

        protected override IMvxAndroidViewPresenter CreateViewPresenter()
        {
            Translation.LoadTranslation();

            var presenter = new MvxFormsDroidPagePresenter();
            //Mvx.RegisterSingleton<IMvxViewPresenter>(presenter);

            var resolverContainer = Resolver.Resolve<IDependencyContainer>();
            resolverContainer.Register<IMvxViewPresenter>(presenter);

            return presenter;
        }
    }
}