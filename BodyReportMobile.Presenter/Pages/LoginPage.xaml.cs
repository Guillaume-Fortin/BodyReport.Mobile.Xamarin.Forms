using BodyReportMobile.Core.ViewModels;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BodyReportMobile.Presenter.Pages
{
	[XamlCompilation (XamlCompilationOptions.Compile)]
	public partial class LoginPage : BaseContentPage
	{
		public LoginPage (LoginViewModel baseViewModel) : base (baseViewModel)
		{
			InitializeComponent ();
		}
	}
}

