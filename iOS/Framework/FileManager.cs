using System;
using System.IO;
using System.Text;
using BodyReportMobile.Core.Framework;

namespace BodyReport.iOS.Framework
{
	public class FileManager : IFileManager
	{
		public String GetResourcesPath()
		{
			//return "Resources" + Path.DirectorySeparatorChar.ToString();
			return "";
			//return Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);
		}

        public bool ResourceFileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        public Stream OpenResourceFile(string filePath)
		{
			return File.Open (filePath, FileMode.Open, FileAccess.Read);
		}

        public String GetDocumentPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public bool FileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        public Stream OpenFile(string filePath)
        {
            return System.IO.File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
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

        public void WriteAllTextFile(string filePath, string contents, Encoding encoding)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            File.WriteAllText(filePath, contents, encoding);
        }

        public bool DeleteFile(string filePath)
        {
            try
            {
                System.IO.File.Delete(filePath);
                return true;
            }
            catch
            {
            }
            return false;
        }

        public bool DirectoryExist(string path)
        {
            try
            {
                return System.IO.Directory.Exists(path);
            }
            catch
            {
            }
            return false;
        }

        public bool CreateDirectory(string path)
        {
            try
            {
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }
    }
}

