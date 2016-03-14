using System;
using Xamarin.Forms;

namespace BodyReport
{
	public class NumericEntry : Entry
	{
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

		public NumericEntry ()
		{
			Completed += NumericEntry_Completed;
		}

		void NumericEntry_Completed (object sender, EventArgs e)
		{
			int value, newValue;
			if (int.TryParse (this.Text, out value)) {
				newValue = value;
				if(MinValue != int.MinValue)
					newValue = Math.Max (newValue, MinValue);
				if(MaxValue != int.MaxValue)
					newValue = Math.Min (newValue, MaxValue);

				if (newValue != value)
					this.Text = newValue.ToString ();
			}
			else
				this.Text = "0";
		}
	}
}

