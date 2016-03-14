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

		public static readonly BindableProperty MinValueProperty = BindableProperty.Create ("MinValue", typeof(int), typeof(NumericEntry), int.MinValue);

		public int MinValue {
			get { return (int)GetValue (MinValueProperty); }
			set { SetValue (MinValueProperty, value); }
		}

		public static readonly BindableProperty MaxValueProperty = BindableProperty.Create ("MaxValue", typeof(int), typeof(NumericEntry), int.MinValue);

		public int MaxValue {
			get { return (int)GetValue (MaxValueProperty); }
			set { SetValue (MaxValueProperty, value); }
		}

		public NumericEntryCell ()
		{
			InitializeComponent ();
		}
	}
}

