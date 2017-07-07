using BodyReport.Message;
using BodyReportMobile.Core.Data;
using System.Collections.Generic;

namespace BodyReportMobile.Core.ServiceLayers
{
    public class CountryService : LocalService
    {
        private const string _cacheName = "CountriesCache";

        public CountryService(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public Country GetCountry(CountryKey key)
        {
            Country country = null;
            string cacheKey = key == null ? "CountryKey_null" : key.GetCacheKey();
            if (key != null && !TryGetCacheData(cacheKey, out country, _cacheName))
            {
                country = GetCountryManager().GetCountry(key);
                SetCacheData(_cacheName, cacheKey, country);
            }
            return country;
        }

        public List<Country> FindCountries(CountryCriteria criteria = null)
        {
            List<Country> countryList = null;
            string cacheKey = criteria == null ? "CountryCriteria_null" : criteria.GetCacheKey();
            if (!TryGetCacheData(cacheKey, out countryList, _cacheName))
            {
                countryList = GetCountryManager().FindCountries(criteria);
                SetCacheData(_cacheName, cacheKey, countryList);
            }
            return countryList;
        }

        public Country UpdateCountry(Country country)
        {
            Country result = null;
            BeginTransaction();
            try
            {
                result = GetCountryManager().UpdateCountry(country);
                CommitTransaction();
                //invalidate cache
                InvalidateCache(_cacheName);
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
                if (countries != null && countries.Count > 0)
                {
                    foreach (var country in countries)
                    {
                        list.Add(GetCountryManager().UpdateCountry(country));
                    }
                    //invalidate cache
                    InvalidateCache(_cacheName);
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
                //invalidate cache
                InvalidateCache(_cacheName);
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
