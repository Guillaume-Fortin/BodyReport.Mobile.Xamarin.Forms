using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Droid.Platform;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.Platform;
using MvvmCross.Droid.Views;
using MvvmCross.Forms.Presenter.Droid;
using MvvmCross.Core.Views;
using MvvmCross.Platform;
using Xamarin.Forms;
using BodyReportMobile.Core;
using Acr.UserDialogs;

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
            AddIocDependencies();
            Translation.LoadTranslation();

            var presenter = new MvxFormsDroidPagePresenter();
            Mvx.RegisterSingleton<IMvxViewPresenter>(presenter);

            return presenter;
        }

        /// <summary>
		/// Adds the ioc dependencies.
		/// </summary>
		private void AddIocDependencies()
        {
            Mvx.RegisterType<ISecurity, SecurityDroid>();
            Mvx.RegisterType<IFileManager, FileManager>();
            Mvx.RegisterType<ISQLite, SQLite_Droid>();
            Mvx.RegisterSingleton<IUserDialogs>(() => UserDialogs.Instance);
        }
    }
}