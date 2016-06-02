using SQLite.Net;

namespace BodyReportMobile.Core.Services
{
    public class LocalService
    {
        /// <summary>
		/// DataBase context with transaction
		/// </summary>
		protected SQLiteConnection _dbContext = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">db context</param>
        public LocalService(SQLiteConnection dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
