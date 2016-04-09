using System;
using BodyReportMobile.Core;

//[assembly: Dependency (typeof (FileManager))] Not Use It because it's PCL dependant with Xamarin.Forms
using System.IO;
using System.Text;
using System.Linq;
using Android.App;
using BodyReportMobile.Core.Framework;

namespace BodyReport.Droid
{
	public class FileManager : IFileManager
	{
		public String GetResourcesPath()
		{
            return "";// Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		}

		public bool FileExist (string filePath)
		{
            return Application.Context.Assets.List("").Where(f => f == filePath).Count() > 0;
		}

		public StreamReader OpenFile (string filePath)
		{
            //var fileStream = System.IO.File.Open (filePath, FileMode.Open);

            var fileStream = Application.Context.Assets.Open(filePath, Android.Content.Res.Access.Streaming);

            return new StreamReader (fileStream);
		}

		public void CloseFile (StreamReader stream)
		{
			if (stream == null) {
				stream.Close ();
				stream.Dispose ();
			}
		}

		public string[] ReadAllLinesFile(string filePath, Encoding encoding)
		{
			return System.IO.File.ReadAllLines (filePath, encoding);
		}

		public string ReadAllTextFile(string filePath, Encoding encoding)
		{
			return System.IO.File.ReadAllText (filePath, encoding);
		}
	}
}

