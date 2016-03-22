using System;
using SQLite.Net;

namespace BodyReportMobile.Core.Crud.Module
{
	public class Crud
	{
		/// <summary>
		/// DataBase context with transaction
		/// </summary>
		protected SQLiteConnection _dbContext = null;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContext">db context</param>
		public Crud(SQLiteConnection dbContext)
		{
			_dbContext = dbContext;
			CreateTable ();
		}

		protected virtual void CreateTable()
		{
		}
	}
}

