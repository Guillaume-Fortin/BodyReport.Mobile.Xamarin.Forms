using System;
using SQLite.Net;

namespace BodyReportMobile.Core.Framework
{
	public interface ISQLite
	{
		SQLiteConnection GetConnection();
	}
}

