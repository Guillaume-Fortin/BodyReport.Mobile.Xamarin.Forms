using BodyReportMobile.Core.Crud.Transformer;
using BodyReportMobile.Core.Models;
using Message;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Crud.Module
{
    public class MuscularGroupModule : Crud
    {
        /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContext">database context</param>
		public MuscularGroupModule(SQLiteConnection dbContext) : base(dbContext)
		{
        }
        
        /// <summary>
        /// Create data in database
        /// </summary>
        /// <param name="muscularGroup">Data</param>
        /// <returns>insert data</returns>
        public MuscularGroup Create(MuscularGroup muscularGroup)
        {
            if (muscularGroup == null || muscularGroup.Id == 0)
                return null;
            
            var muscularGroupRow = new MuscularGroupRow();
            MuscularGroupTransformer.ToRow(muscularGroup, muscularGroupRow);
            _dbContext.Insert(muscularGroupRow);
            return MuscularGroupTransformer.ToBean(muscularGroupRow);
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public MuscularGroup Get(MuscularGroupKey key)
        {
            if (key == null || key.Id == 0)
                return null;

            var muscularGroupRow = _dbContext.Table<MuscularGroupRow>().Where(m => m.Id == key.Id).FirstOrDefault();
            if (muscularGroupRow != null)
            {
                return MuscularGroupTransformer.ToBean(muscularGroupRow);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<MuscularGroup> Find(MuscularGroupCriteria muscularGroupCriteria = null)
        {
            List<MuscularGroup> resultList = null;
            TableQuery<MuscularGroupRow> muscularGroupRowList = _dbContext.Table<MuscularGroupRow>();
            CriteriaTransformer.CompleteQuery(ref muscularGroupRowList, muscularGroupCriteria);

            if (muscularGroupRowList != null && muscularGroupRowList.Count() > 0)
            {
                resultList = new List<MuscularGroup>();
                foreach (var muscularGroupRow in muscularGroupRowList)
                {
                    resultList.Add(MuscularGroupTransformer.ToBean(muscularGroupRow));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Update data in database
        /// </summary>
        /// <param name="muscularGroup">data</param>
        /// <returns>updated data</returns>
        public MuscularGroup Update(MuscularGroup muscularGroup)
        {
            if (muscularGroup == null || muscularGroup.Id == 0)
                return null;

            var muscularGroupRow = _dbContext.Table<MuscularGroupRow>().Where(m => m.Id == muscularGroup.Id).FirstOrDefault();
            if (muscularGroupRow == null)
            { // No data in database
                return Create(muscularGroup);
            }
            else
            { //Modify Data in database
                MuscularGroupTransformer.ToRow(muscularGroup, muscularGroupRow);
                return MuscularGroupTransformer.ToBean(muscularGroupRow);
            }
        }

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        public void Delete(MuscularGroupKey key)
        {
            if (key == null || key.Id == 0)
                return;

            var muscularGroupRow = _dbContext.Table<MuscularGroupRow>().Where(m => m.Id == key.Id).FirstOrDefault();
            if (muscularGroupRow != null)
            {
                _dbContext.Delete(muscularGroupRow);
            }
        }
    }
}
