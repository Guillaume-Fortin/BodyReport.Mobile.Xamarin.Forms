using BodyReportMobile.Core.Crud.Module;
using BodyReportMobile.Core.Framework;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using BodyReportMobile.Core.Data;

namespace BodyReportMobile.Core.Manager
{
    public class CountryManager : BodyReportManager
    {
        CountryModule _module;
        public CountryManager(ApplicationDbContext dbContext) : base(dbContext)
		{
            _module = new CountryModule(DbContext);
        }

        public List<Country> FindCountries(CountryCriteria countryCriteria = null)
        {
            return _module.Find(countryCriteria);
        }

        public Country GetCountry(CountryKey key)
        {
            return _module.Get(key);
        }

        public Country UpdateCountry(Country country)
        {
            return _module.Update(country);
        }

        public void DeleteCountry(Country country)
        {
            _module.Delete(country);
        }
    }
}
