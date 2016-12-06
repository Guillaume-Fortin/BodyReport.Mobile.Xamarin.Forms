﻿using System;
using BodyReportMobile.Core;

//[assembly: Dependency (typeof (FileManager))] Not Use It because it's PCL dependant with Xamarin.Forms
using System.IO;
using System.Text;
using System.Linq;
using Android.App;
using BodyReportMobile.Core.Framework;
using System.Threading.Tasks;

namespace BodyReport.Droid
{
	public class FileManager : IFileManager
	{
		public String GetResourcesPath()
		{
            return "";// Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		}

        public bool ResourceFileExist (string filePath)
		{
            return Application.Context.Assets.List("").Where(f => f == filePath).Count() > 0;
		}

        public Stream OpenResourceFile(string filePath)
        {
            return  Application.Context.Assets.Open(filePath, Android.Content.Res.Access.Streaming);
        }

        public String GetDocumentPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public bool FileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        public Stream OpenFile (string filePath)
		{
            return System.IO.File.Open (filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
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

        public async Task<bool> WriteBinaryFileAsync(string filePath, MemoryStream memoryStream)
        {
            bool result = false;
            try
            {
                if (memoryStream == null || memoryStream.Length == 0)
                    return false;
                if (memoryStream.Position != 0)
                    memoryStream.Position = 0;
                if (File.Exists(filePath))
                    File.Delete(filePath);
                using (var fs = File.OpenWrite(filePath))
                {
                    await memoryStream.CopyToAsync(fs);
                }
                return true;
            }
            catch(Exception except)
            {
            }
            return result;
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

        public bool CopyFile(string sourceFileName, string destFileName, bool overrideDestFile = true)
        {
            try
            {
                if (File.Exists(sourceFileName))
                {
                    if (File.Exists(destFileName))
                    {
                        if (!overrideDestFile)
                            return false;

                        File.Delete(destFileName);
                    }

                    File.Copy(sourceFileName, destFileName);
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

