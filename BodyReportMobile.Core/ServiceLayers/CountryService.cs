using BodyReport.Message;
using BodyReportMobile.Core.Manager;
using SQLite.Net;
using System.Collections.Generic;

namespace BodyReportMobile.Core.ServiceLayers
{
    public class CountryService : LocalService
    {
        public CountryService(SQLiteConnection dbContext) : base(dbContext)
        {
        }

        public List<Country> FindCountries(CountryCriteria countryCriteria = null)
        {
            return GetCountryManager().FindCountries(countryCriteria);
        }

        public Country GetCountry(CountryKey key)
        {
            return GetCountryManager().GetCountry(key);
        }

        public Country UpdateCountry(Country country)
        {
            Country result = null;
            BeginTransaction();
            try
            {
                result = GetCountryManager().UpdateCountry(country);
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                EndTransaction();
            }

            return result;
        }

        public List<Country> UpdateCountryList(List<Country> countries)
        {
            if (countries == null)
                return null;

            List<Country> list = new List<Country>();
            BeginTransaction();
            try
            {
                foreach (var country in countries)
                {
                    list.Add(GetCountryManager().UpdateCountry(country));
                }
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                EndTransaction();
            }

            return list;
        }

        public void DeleteCountry(Country country)
        {
            BeginTransaction();
            try
            {
                GetCountryManager().DeleteCountry(country);
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                EndTransaction();
            }
        }
    }
}
