using System;
using Xamarin.Forms;
using System.Globalization;
using Framework;

namespace BodyReportMobile.Presenter.Framework.Controls
{
	public class NumericEntry : Entry
	{
		public static readonly BindableProperty MinValueProperty = BindableProperty.Create ("MinValue", typeof(double), typeof(NumericEntry), double.MinValue);

		public double MinValue {
			get { return (double)GetValue (MinValueProperty); }
			set { SetValue (MinValueProperty, value); }
		}

		public static readonly BindableProperty MaxValueProperty = BindableProperty.Create ("MaxValue", typeof(double), typeof(NumericEntry), double.MinValue);

		public double MaxValue {
			get { return (double)GetValue (MaxValueProperty); }
			set { SetValue (MaxValueProperty, value); }
		}

		public static readonly BindableProperty IsIntegerProperty = BindableProperty.Create ("IsInteger", typeof(bool), typeof(NumericEntry), false);

		public bool IsInteger {
			get { return (bool)GetValue (IsIntegerProperty); }
			set { SetValue (IsIntegerProperty, value); }
		}

		public NumericEntry ()
		{
			Completed += NumericEntry_Completed;
		}

		void NumericEntry_Completed (object sender, EventArgs e)
		{
			if(IsInteger)
				this.Text = ParseIntegerValue ().ToString();
			else
				this.Text = ParseDoubleValue ().ToString();
		}

		private double ParseDoubleValue()
		{
			double value, newValue;
			if (Utils.TryParse (this.Text, out value)) {
				newValue = value;
				if(MinValue != double.MinValue)
					newValue = Math.Max (newValue, MinValue);
				if(MaxValue != double.MaxValue)
					newValue = Math.Min (newValue, MaxValue);

				return newValue;
			}
			else
				return MinValue;
		}

		private int ParseIntegerValue()
		{
			int minValue = int.MinValue;
			if (MinValue > int.MinValue) //check Min double for integer value
				minValue = (int)MinValue;

			int maxValue = int.MaxValue;
			if (MaxValue < int.MaxValue)//check Max double for integer value
				maxValue = (int)MaxValue;
			
			int value, newValue;
			if (int.TryParse (this.Text, out value)) {
				newValue = value;
				if(minValue != int.MinValue)
					newValue = Math.Max (newValue, minValue);
				if(maxValue != int.MaxValue)
					newValue = Math.Min (newValue, maxValue);

				return newValue;
			}
			else
				return minValue;
		}
	}
}

