using BodyReportMobile.Core.Crud.Transformer;
using BodyReportMobile.Core.Models;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using BodyReportMobile.Core.Data;

namespace BodyReportMobile.Core.Crud.Module
{
    public class CountryModule : Crud
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">database context</param>
        public CountryModule(ApplicationDbContext dbContext) : base(dbContext)
		{
        }

        /// <summary>
		/// Create data in database
		/// </summary>
		/// <param name="country">Data</param>
		/// <returns>insert data</returns>
		public Country Create(Country country)
        {
            if (country == null || country.Id == 0)
                return null;

            var row = new CountryRow();
            CountryTransformer.ToRow(country, row);
            _dbContext.Country.Add(row);
            _dbContext.SaveChanges();
            return CountryTransformer.ToBean(row);
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public Country Get(CountryKey key)
        {
            if (key == null || key.Id == 0)
                return null;

            var row = _dbContext.Country.Where(m => m.Id == key.Id).FirstOrDefault();
            if (row != null)
            {
                return CountryTransformer.ToBean(row);
            }
            return null;
        }

        /// <summary>
        /// Find data in database
        /// </summary>
        /// <returns></returns>
        public List<Country> Find(CountryCriteria countryCriteria = null)
        {
            List<Country> resultList = null;
            IQueryable<CountryRow> rowList = _dbContext.Country;
            CriteriaTransformer.CompleteQuery(ref rowList, countryCriteria);

            if (rowList != null)
            {
                resultList = new List<Country>();
                foreach (var row in rowList)
                {
                    if (resultList == null)
                        resultList = new List<Country>();
                    resultList.Add(CountryTransformer.ToBean(row));
                }
            }
            return resultList;
        }

        /// <summary>
		/// Update data in database
		/// </summary>
		/// <param name="muscle">data</param>
		/// <returns>updated data</returns>
		public Country Update(Country muscle)
        {
            if (muscle == null || muscle.Id == 0)
                return null;

            var row = _dbContext.Country.Where(m => m.Id == muscle.Id).FirstOrDefault();
            if (row == null)
            { // No data in database
                return Create(muscle);
            }
            else
            { //Modify Data in database
                CountryTransformer.ToRow(muscle, row);
                _dbContext.SaveChanges();
                return CountryTransformer.ToBean(row);
            }
        }

        /// <summary>
		/// Delete data in database
		/// </summary>
		/// <param name="key">Primary Key</param>
		public void Delete(CountryKey key)
        {
            if (key == null || key.Id == 0)
                return;

            var row = _dbContext.Country.Where(m => m.Id == key.Id).FirstOrDefault();
            if (row != null)
            {
                _dbContext.Country.Remove(row);
                _dbContext.SaveChanges();
            }
        }
    }
}
