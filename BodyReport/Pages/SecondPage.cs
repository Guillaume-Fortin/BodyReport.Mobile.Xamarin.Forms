using System;

using Xamarin.Forms;

namespace BodyReport
{
	public class SecondPage : BaseContentPage
	{
		public SecondPage () : base()
		{
			var btn = new Button { Text = "Go to Third page" };
			btn.SetBinding(Button.CommandProperty, new Binding("DisplayViewCommand"));

			Title = "Second Page";
			Content = new StackLayout { 
				Children = {
					new Label { Text = "Second Page" },
					btn
				}
			};
		}
	}
}


