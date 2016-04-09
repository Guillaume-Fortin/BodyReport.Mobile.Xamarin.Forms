using System;

using Xamarin.Forms;

namespace BodyReportMobile.Presenter.Pages
{
	public class ThirdPage : BaseContentPage
	{
		public ThirdPage () : base()
		{
			Title = "Third Page";
			Content = new StackLayout { 
				Children = {
					new Label { Text = "Third Page" }
				}
			};
		}
	}
}


