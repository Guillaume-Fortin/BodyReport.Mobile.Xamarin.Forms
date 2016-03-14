using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace BodyReport
{
	public partial class NumericEntryCell : ViewCell
	{
		public static readonly BindableProperty LabelProperty = BindableProperty.Create ("Label", typeof(string), typeof(TouchViewCell), "");

		public string Label {
			get { return (string)GetValue (LabelProperty); }
			set { SetValue (LabelProperty, value); }
		}

		public static readonly BindableProperty TextProperty = BindableProperty.Create ("Text", typeof(string), typeof(TouchViewCell), "");

		public string Value {
			get { return (string)GetValue (TextProperty); }
			set { SetValue (TextProperty, value); }
		}

		public NumericEntryCell ()
		{
			InitializeComponent ();
		}
	}
}

