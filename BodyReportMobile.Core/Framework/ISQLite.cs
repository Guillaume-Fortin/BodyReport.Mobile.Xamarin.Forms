using System;
using SQLite.Net;

namespace BodyReportMobile.Core
{
	public interface ISQLite
	{
		SQLiteConnection GetConnection();
	}
}

