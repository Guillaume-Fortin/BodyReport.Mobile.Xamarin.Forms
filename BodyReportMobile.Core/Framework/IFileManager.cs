using System;
using System.IO;
using System.Text;

namespace BodyReportMobile.Core.Framework
{
	public interface IFileManager
	{
		String GetResourcesPath();
		bool FileExist (string filePath);
		StreamReader OpenFile (string filePath);
		void CloseFile (StreamReader stream);
		string[] ReadAllLinesFile(string filePath, Encoding encoding);
		string ReadAllTextFile(string filePath, Encoding encoding);
	}
}

