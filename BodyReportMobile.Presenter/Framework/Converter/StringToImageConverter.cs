using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BodyReportMobile.Core.Framework;
using Xamarin.Forms;
using XLabs.Ioc;

namespace BodyReportMobile.Presenter.Framework.Converter
{
    public class StringToImageConverter : IValueConverter
    {
		private static IFileManager _fileManager = null;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string && !string.IsNullOrWhiteSpace((string)value))
            {
                var filename = (string)value;

				if(_fileManager == null)
					_fileManager = Resolver.Resolve<IFileManager> ();
				if (_fileManager.FileExist (filename) && _fileManager.FileLength (filename) == 0) //ios security
				{
					_fileManager.DeleteFile (filename);
					return null;
				}
				else
				{
					return ImageSource.FromFile (filename);
				}
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
