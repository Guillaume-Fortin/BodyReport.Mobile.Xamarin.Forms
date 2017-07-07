using System;
using BodyReportMobile.Core.Data;

namespace BodyReportMobile.Core.Manager
{
	public class BodyReportManager
	{
		/// <summary>
		/// DataBase context with transaction
		/// </summary>
		private ApplicationDbContext _dbContext = null;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContext">db context</param>
		public BodyReportManager(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

        public ApplicationDbContext DbContext
        {
            get
            {
                return _dbContext;
            }
        }
    }
}

