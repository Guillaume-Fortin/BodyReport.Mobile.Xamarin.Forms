using System;
using System.IO;
using System.Text;
using BodyReportMobile.Core.Framework;

namespace BodyReport.iOS
{
	public class FileManager : IFileManager
	{
		public String GetResourcesPath()
		{
			//return "Resources" + Path.DirectorySeparatorChar.ToString();
			return "";
			//return Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);
		}

		public bool FileExist (string filePath)
		{
			return System.IO.File.Exists (filePath);
		}

		public StreamReader OpenFile (string filePath)
		{
			var fileStream = System.IO.File.Open (filePath, FileMode.Open);
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

