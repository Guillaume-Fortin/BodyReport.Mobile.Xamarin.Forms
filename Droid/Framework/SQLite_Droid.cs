using System;
using BodyReportMobile.Core;
//using Xamarin.Forms;

//[assembly: Dependency (typeof (SQLite_iOS))]
using SQLite.Net;
using System.IO;


namespace BodyReport.Droid
{
	public class SQLite_Droid : ISQLite
	{
		public SQLite_Droid()
		{
		}

		public SQLiteConnection GetConnection()
		{
			var sqliteFilename = "bodyreport.db3";

            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, sqliteFilename);
            // Create the connection
            var conn = new SQLiteConnection(new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid(), path);
            // Return the database connection
            return conn;
        }
	}
}

