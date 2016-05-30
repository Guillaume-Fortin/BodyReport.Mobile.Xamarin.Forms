using BodyReportMobile.Core.Crud.Transformer;
using BodyReportMobile.Core.Models;
using BodyReport.Message;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Crud.Module
{
    public class BodyExerciseModule : Crud
    {
        /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContext">database context</param>
		public BodyExerciseModule(SQLiteConnection dbContext) : base(dbContext)
		{
        }

        /// <summary>
        /// Create data in database
        /// </summary>
        /// <param name="bodyExercise">Data</param>
        /// <returns>insert data</returns>
        public BodyExercise Create(BodyExercise bodyExercise)
        {
            if (bodyExercise == null)
                return null;
            
            if (bodyExercise.Id == 0)
                return null;

            var bodyExerciseRow = new BodyExerciseRow();
            BodyExerciseTransformer.ToRow(bodyExercise, bodyExerciseRow);
            _dbContext.Insert(bodyExerciseRow);
            return BodyExerciseTransformer.ToBean(bodyExerciseRow);
        }

        /// <summary>
        /// Get data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        /// <returns>read data</returns>
        public BodyExercise Get(BodyExerciseKey key)
        {
            if (key == null || key.Id == 0)
                return null;

            var bodyExerciseRow = _dbContext.Table<BodyExerciseRow>().Where(m => m.Id == key.Id).FirstOrDefault();
            if (bodyExerciseRow != null)
            {
                return BodyExerciseTransformer.ToBean(bodyExerciseRow);
            }
            return null;
        }

        /// <summary>
        /// Find body exercises
        /// </summary>
        /// <returns></returns>
        public List<BodyExercise> Find(BodyExerciseCriteria bodyExerciseCriteria = null)
        {
            List<BodyExercise> resultList = null;
            TableQuery<BodyExerciseRow> muscularGroupRowList = _dbContext.Table<BodyExerciseRow>();
            CriteriaTransformer.CompleteQuery(ref muscularGroupRowList, bodyExerciseCriteria);

            if (muscularGroupRowList != null && muscularGroupRowList.Count() > 0)
            {
                resultList = new List<BodyExercise>();
                foreach (var muscularGroupRow in muscularGroupRowList)
                {
                    resultList.Add(BodyExerciseTransformer.ToBean(muscularGroupRow));
                }
            }
            return resultList;
        }

        /// <summary>
        /// Update data in database
        /// </summary>
        /// <param name="bodyExercise">data</param>
        /// <returns>updated data</returns>
        public BodyExercise Update(BodyExercise bodyExercise)
        {
            if (bodyExercise == null || bodyExercise.Id == 0)
                return null;

            var bodyExerciseRow = _dbContext.Table<BodyExerciseRow>().Where(m => m.Id == bodyExercise.Id).FirstOrDefault();
            if (bodyExerciseRow == null)
            { // No data in database
                return Create(bodyExercise);
            }
            else
            { //Modify Data in database
                BodyExerciseTransformer.ToRow(bodyExercise, bodyExerciseRow);
                _dbContext.Delete(bodyExerciseRow); //Update don't work... need delete and insert
                _dbContext.Insert(bodyExerciseRow);
                return BodyExerciseTransformer.ToBean(bodyExerciseRow);
            }
        }

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="key">Primary Key</param>
        public void Delete(BodyExerciseKey key)
        {
            if (key == null || key.Id == 0)
                return;

            var bodyExerciseRow = _dbContext.Table<BodyExerciseRow>().Where(m => m.Id == key.Id).FirstOrDefault();
            if (bodyExerciseRow != null)
            {
                _dbContext.Delete(bodyExerciseRow);
            }
        }
    }
}
