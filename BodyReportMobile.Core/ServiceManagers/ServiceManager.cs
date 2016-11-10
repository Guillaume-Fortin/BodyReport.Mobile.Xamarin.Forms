using System;
using SQLite.Net;

namespace BodyReportMobile.Core.ServiceManagers
{
	public class ServiceManager
	{
		/// <summary>
		/// DataBase context with transaction
		/// </summary>
		protected SQLiteConnection _dbContext = null;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContext">db context</param>
		public ServiceManager(SQLiteConnection dbContext)
		{
			_dbContext = dbContext;
		}
	}
}

