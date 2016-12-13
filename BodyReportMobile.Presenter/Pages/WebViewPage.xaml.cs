using System;
using System.Collections.Generic;
using BodyReportMobile.Core;
using BodyReportMobile.Presenter.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BodyReportMobile.Presenter
{
	[XamlCompilation (XamlCompilationOptions.Compile)]
	public partial class WebViewPage : BaseContentPage
	{
		public WebViewPage (WebViewViewModel baseViewModel) : base (baseViewModel)
		{
			InitializeComponent ();
		}

		void PrintButton_Clicked (object sender, System.EventArgs e)
		{
			(_viewModel as WebViewViewModel)?.PrintCommand.Execute (this.webView);
		}
	}
}
