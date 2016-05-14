using System;
using SQLite.Net;
using System.IO;
using BodyReportMobile.Core.Framework;

namespace BodyReport.iOS.Framework
{
	public class SQLite_iOS : ISQLite
	{	
		public SQLite_iOS ()
		{
		}

		public SQLiteConnection GetConnection()
		{
			var sqliteFilename = "bodyreport.db3";
			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
			string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
			var path = Path.Combine(libraryPath, sqliteFilename);
			// Create the connection
			var conn = new SQLiteConnection(new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS(), path);
			// Return the database connection
			return conn;
		}
	}
}

