using System;
using SQLite.Net;

namespace BodyReportMobile.Core.Manager
{
	public class BodyReportManager
	{
		/// <summary>
		/// DataBase context with transaction
		/// </summary>
		protected SQLiteConnection _dbContext = null;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContext">db context</param>
		public BodyReportManager(SQLiteConnection dbContext)
		{
			_dbContext = dbContext;
		}
	}
}

