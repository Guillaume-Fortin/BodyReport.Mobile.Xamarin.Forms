using System;
using MvvmCross.iOS.Platform;
using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Views.Presenters;
using UIKit;
using MvvmCross.Platform;
using BodyReportMobile.Core;
using MvvmCross.Platform.Platform;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using MvvmCross.Core.Views;
using Xamarin.Forms;
using MvvmCross.Forms.Presenter.iOS;
using MvvmCross.Forms.Presenter.Core;
using BodyReport.Pages;

namespace BodyReport.iOS
{
	public class Setup : MvxIosSetup
	{
		public Setup(MvxApplicationDelegate applicationDelegate, UIWindow window)
			: base(applicationDelegate, window)
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

		protected override IMvxIosViewPresenter CreatePresenter()
		{
			Forms.Init();

			var xamarinFormsApp = new MvxFormsApp();

			return new MvxFormsIosPagePresenter(Window, xamarinFormsApp);
		}

		/*
		protected override IEnumerable<Assembly> GetViewModelAssemblies()
		{
			var result = base.GetViewModelAssemblies();
			var assemblyList = result.ToList();
			assemblyList.Add(typeof(TipViewModel).Assembly);
			return assemblyList.ToArray();
		}

		protected override IEnumerable<Assembly> GetViewAssemblies()
		{
			var result = base.GetViewAssemblies();
			var assemblyList = result.ToList();
			assemblyList.Add(typeof(TipPage).Assembly);
			return assemblyList.ToArray();
		}

		protected override void InitializeViewLookup()
		{
			base.InitializeViewLookup();

			var vmLookup = new Dictionary<Type, Type> {
				{typeof (TipViewModel), typeof (TipPage)}
			};

			var container = Mvx.Resolve<IMvxViewsContainer>();
			container.AddAll(vmLookup);
		}
		*/
	}

}

