using BodyReportMobile.Core.ViewModels;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace BodyReportMobile.Presenter.Pages
{
	public partial class LoginPage : BaseContentPage
	{
		public LoginPage (LoginViewModel baseViewModel) : base (baseViewModel)
		{
			InitializeComponent ();
		}
	}
}

