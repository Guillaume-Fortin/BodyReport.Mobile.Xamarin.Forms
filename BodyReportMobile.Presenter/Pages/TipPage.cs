using System;

using Xamarin.Forms;

namespace BodyReportMobile.Presenter.Pages
{
	public class TipPage : BaseContentPage
	{
		public TipPage () : base()
		{
			DisableBackButton = true;

			var btn = new Button { Text = "Go to second page" };
			btn.SetBinding(Button.CommandProperty, new Binding("DisplaySecondPageCommand"));

			Title = "Fist Page";
			Content = new ScrollView(){ 
				VerticalOptions = LayoutOptions.FillAndExpand,
				Content = new StackLayout { 
				Children = {
					new Label { Text = "Hello ContentPage" },
					btn
				}
			}};
		}
	}
}


