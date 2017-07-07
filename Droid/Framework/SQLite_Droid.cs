using System;
using System.IO;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Data;

namespace BodyReport.Droid
{
	public class SQLite_Droid : ISQLite
	{
		public SQLite_Droid()
		{
		}

		public ApplicationDbContext GetConnection()
		{
			var sqliteFilename = "bodyreport.db";

            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, sqliteFilename);
            // Create the connection
            var conn = new ApplicationDbContext(path);
            // Return the database connection
            return conn;
        }
	}
}

