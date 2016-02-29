using System;
using Xamarin.Forms;
using System.Text;
using System.Globalization;
using System.Linq;

namespace BodyReport
{
	public class TextToIntConverter : IValueConverter
	{
		private char[] okValues = {'0','1','2','3','4','5','6','7','8','9'};

		public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return string.Empty;

			return value.ToString ();
		} 

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string)
			{
				var str = value as string;
				int result;
				if (int.TryParse (str, out result))
					return result;

				StringBuilder sb = new StringBuilder ();
				foreach (char c in str) {
					if (okValues.Contains (c))
						sb.Append (c);
				}

				str = sb.ToString ();

				if (str.Length == 0)
					return 0;

				if (int.TryParse (str, out result))
					return result;

				do {
					str = str.Remove(str.Length -1);
				} while(str.Length >0 && !int.TryParse (str, out result));

				return str;
			}

			return 0;
		}
	}
}

