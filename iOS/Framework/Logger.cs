using System;
using BodyReportMobile.Core.Framework;
using System.IO;
using System.Text;

namespace BodyReport.iOS.Framework
{
	public class Logger : ILogger
	{
		private object _locker = new object();
		private string _fileName = "BodyReport.log";
		private bool _firstWrite = true;

		public Logger ()
		{
		}

		protected override void LogInFile(string message)
		{
			lock (_locker)
			{
				try
				{
					string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), _fileName);
					if (_firstWrite && File.Exists(filePath))
					{
						_firstWrite = false;
						File.Delete(filePath);
					}

					using (var streamWriter = new StreamWriter(filePath, true, Encoding.UTF8))
					{
						streamWriter.Write(message);
					}
				}
				catch (Exception except)
				{
					Console.WriteLine("BodyReport", except.ToString());
				}
			}
		}
	}
}

