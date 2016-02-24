using System;
using MvvmCross.Platform.IoC;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using BodyReportMobile.Core.Services;
using MvvmCross.Forms.Presenter.Core;

namespace BodyReport
{
	public class App: MvxApplication
	{
		public App()
		{
			Mvx.RegisterType<ICalculation,Calculation>();
		}

		public override void Initialize()
		{
			CreatableTypes()
				.EndingWith("Service")
				.AsInterfaces()
				.RegisterAsLazySingleton();

			//Need for locate Xamarin.Forms Page
			Mvx.RegisterSingleton(typeof(IMvxFormsPageLoader), new MyFormsPageLoader());

			RegisterAppStart<BodyReportMobile.Core.ViewModels.MainViewModel>();
		}
	}
}

