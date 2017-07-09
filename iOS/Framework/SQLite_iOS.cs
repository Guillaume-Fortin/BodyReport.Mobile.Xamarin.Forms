using System;
using System.IO;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Data;

namespace BodyReport.iOS.Framework
{
	public class SQLite_iOS : ISQLite
	{	
		public SQLite_iOS ()
		{
		}

		public ApplicationDbContext GetConnection()
		{
			var sqliteFilename = "bodyreport.db";
			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
			string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
			var path = Path.Combine(libraryPath, sqliteFilename);
			// Create the connection
            var conn = new ApplicationDbContext(path);
            // Return the database connection
            return conn;
		}
	}
}

