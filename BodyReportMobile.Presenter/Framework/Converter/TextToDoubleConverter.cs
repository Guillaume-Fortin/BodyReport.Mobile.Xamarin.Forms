using System;
using Xamarin.Forms;
using System.Text;
using System.Globalization;
using System.Linq;

namespace BodyReportMobile.Presenter.Framework.Converter
{
    public class TextToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
                return ((double)value).ToString(CultureInfo.InvariantCulture);

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double resultDouble;
            string dotValue = value as string;
            if (dotValue != null && dotValue.IndexOf(',') != -1)
                dotValue = dotValue.Replace(',', '.');
            if (dotValue.Length > 0 && dotValue[dotValue.Length - 1] == '.')
                return value;
            if (double.TryParse(dotValue, NumberStyles.Any, CultureInfo.InvariantCulture, out resultDouble))
            {
                return resultDouble;
            }
            return value;
        }
    }
}

