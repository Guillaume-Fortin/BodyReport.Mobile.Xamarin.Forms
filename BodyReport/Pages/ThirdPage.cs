using System;

using Xamarin.Forms;

namespace BodyReport
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


