using System;
using Xamarin.Forms;
using System.Text;
using System.Globalization;
using System.Linq;

namespace BodyReportMobile.Presenter.Framework.Converter
{
    public class TextToDoubleConverter : IValueConverter
    {
        private char[] okValues = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ',', '.' };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
                return value.ToString();

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double resultDouble;
            string dotValue = value as string;
            if (dotValue != null && dotValue.IndexOf(',') != -1)
                dotValue = dotValue.Replace(',', '.');
            if (double.TryParse(dotValue, NumberStyles.Any, CultureInfo.InvariantCulture, out resultDouble))
            {
                return resultDouble;
            }
            return value;
        }
    }
}

