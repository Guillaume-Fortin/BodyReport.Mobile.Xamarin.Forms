using System;
using MvvmCross.Platform.IoC;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Forms.Presenter.Core;
using BodyReportMobile.Core;
using XLabs.Ioc;
using BodyReportMobile.Core.ViewModels;

namespace BodyReportMobile.Presenter
{
	public class App: MvxApplication
	{
		public App()
		{
		}

		public override void Initialize()
		{
			CreatableTypes()
				.EndingWith("Service")
				.AsInterfaces()
				.RegisterAsLazySingleton();

            //Need for locate Xamarin.Forms Page
            var resolverContainer = Resolver.Resolve<IDependencyContainer>();
            resolverContainer.Register<IMvxFormsPageLoader>(new MyFormsPageLoader());
            //Mvx.RegisterSingleton(typeof(IMvxFormsPageLoader), new MyFormsPageLoader());

            resolverContainer.Register<BaseViewModel, BodyReportMobile.Core.ViewModels.MainViewModel>();
		}
	}
}

