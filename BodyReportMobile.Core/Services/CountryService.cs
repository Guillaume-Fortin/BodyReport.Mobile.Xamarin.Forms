using BodyReport.Message;
using BodyReportMobile.Core.ServiceManagers;
using SQLite.Net;
using System.Collections.Generic;

namespace BodyReportMobile.Core.Services
{
    public class CountryService : LocalService
    {
        CountryManager _manager;

        public CountryService(SQLiteConnection dbContext) : base(dbContext)
        {
            _manager = new CountryManager(_dbContext);
        }

        public List<Country> FindCountries(CountryCriteria countryCriteria = null)
        {
            return _manager.FindCountries(countryCriteria);
        }

        public Country GetCountry(CountryKey key)
        {
            return _manager.GetCountry(key);
        }

        public Country UpdateCountry(Country country)
        {
            return _manager.UpdateCountry(country);
        }

        public List<Country> UpdateCountryList(List<Country> countries)
        {
            if (countries == null)
                return null;

            List<Country> list = new List<Country>();
            _dbContext.BeginTransaction();
            try
            {
                foreach (var country in countries)
                {
                    list.Add(_manager.UpdateCountry(country));
                }
            }
            catch
            {
                _dbContext.Rollback();
                throw;
            }
            finally
            {
                _dbContext.Commit();
            }

            return list;
        }

        public void DeleteCountry(Country country)
        {
            _manager.DeleteCountry(country);
        }
    }
}
