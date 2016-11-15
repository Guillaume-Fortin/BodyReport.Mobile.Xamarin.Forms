using System;
using Xamarin.Forms;
using System.Globalization;
using BodyReport.Framework;

namespace BodyReportMobile.Presenter.Framework.Controls
{
	public class NumericEntry : Entry
	{
        public static readonly BindableProperty MinValueProperty = BindableProperty.Create ("MinValue", typeof(double), typeof(NumericEntry), double.MinValue);

        private string _oldText = null;
        private bool _focused = false;

		public double MinValue {
			get { return (double)GetValue (MinValueProperty); }
			set { SetValue (MinValueProperty, value); }
		}

		public static readonly BindableProperty MaxValueProperty = BindableProperty.Create ("MaxValue", typeof(double), typeof(NumericEntry), double.MaxValue);

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
            Keyboard = Keyboard.Numeric;
            //TODO binding necessary?
            this.SetBinding(Entry.TextProperty, new Binding(path: "Text", source: this));

            Completed += NumericEntry_Completed;
            Focused += NumericEntry_Focused;
            Unfocused += NumericEntry_Unfocused;
        }

        private void NumericEntry_Focused(object sender, FocusEventArgs e)
        {
            if (sender == this)
            {
                if(!_focused)
                    _focused = true;
                _oldText = this.Text;
            }       
        }

        private void NumericEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if(sender == this && _focused)
            {
                _focused = false;
                CheckValue();
            }   
        }

        void NumericEntry_Completed(object sender, EventArgs e)
        {
            if (sender == this)
                CheckValue();
        }

        private void CheckValue()
        {
            if (_oldText != this.Text)
            {
                string newText;
                double newValue;
                if (IsInteger)
                {
                    newText = ParseIntegerValue().ToString();
                    if (newText != this.Text)
                    {
                        this.Text = "";
                        this.Text = newText;
                    }
                }
                else if (ParseDoubleValue(this.Text, out newValue))
                {
                    newText = newValue.ToString(CultureInfo.InvariantCulture);
                    if (newText != this.Text)
                    {
                        this.Text = "";
                        this.Text = newText;
                    }
                }
                _oldText = this.Text;
            }
		}

		private bool ParseDoubleValue(string text, out double result)
		{
            result = MinValue;
            if (!string.IsNullOrEmpty(text) && (text[text.Length - 1] == '.' || text[text.Length - 1] == '|'))
                return false;

            double value, newValue;
			if ( Utils.TryParse (text, out value)) {
				newValue = value;
				if(MinValue != double.MinValue)
					newValue = Math.Max (newValue, MinValue);
				if(MaxValue != double.MaxValue)
					newValue = Math.Min (newValue, MaxValue);
                
                result = newValue;
                return true;
			}
			else
            {
                result = MinValue;
                return true;
            }
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

