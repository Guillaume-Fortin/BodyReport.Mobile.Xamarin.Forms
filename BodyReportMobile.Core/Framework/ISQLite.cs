using System;
using BodyReportMobile.Core.Data;

namespace BodyReportMobile.Core.Framework
{
	public interface ISQLite
	{
        ApplicationDbContext GetConnection();
	}
}

