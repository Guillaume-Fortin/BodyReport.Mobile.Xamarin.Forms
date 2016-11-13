using BodyReportMobile.Core.Crud.Module;
using BodyReportMobile.Core.Crud.Transformer;
using BodyReportMobile.Core.Framework;
using BodyReport.Message;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyReportMobile.Core.Manager
{
    public class BodyExerciseManager : BodyReportManager
    {
        BodyExerciseModule _module = null;

        public BodyExerciseManager(SQLiteConnection dbContext) : base(dbContext)
        {
            _module = new BodyExerciseModule(DbContext);
        }

        public BodyExercise Get(BodyExerciseKey key)
        {
            var bodyExercise = _module.Get(key);
            if (bodyExercise != null)
                bodyExercise.Name = Translation.GetInDB(BodyExerciseTransformer.GetTranslationKey(bodyExercise.Id));
            return bodyExercise;
        }

        public List<BodyExercise> Find(BodyExerciseCriteria bodyExerciseCriteria = null)
        {
            var bodyExerciseList = _module.Find(bodyExerciseCriteria);
            if (bodyExerciseList != null)
            {
                foreach (var bodyExercise in bodyExerciseList)
                {
                    if (bodyExercise != null)
                        bodyExercise.Name = Translation.GetInDB(BodyExerciseTransformer.GetTranslationKey(bodyExercise.Id));
                }
            }

            return bodyExerciseList;
        }

        public BodyExercise Create(BodyExercise bodyExercise)
        {
            string name = bodyExercise.Name;
            bodyExercise = _module.Create(bodyExercise);
            if (bodyExercise != null && bodyExercise.Id > 0)
            {
                //Update Translation Name
                string trKey = BodyExerciseTransformer.GetTranslationKey(bodyExercise.Id);
                /*Translation.UpdateInDB(trKey, name, _dbContext);*/
                bodyExercise.Name = Translation.GetInDB(trKey);
            }
            return bodyExercise;
        }

        internal void Delete(BodyExerciseKey key)
        {
            //Update Translation Name
            //Translation.DeleteInDB(MuscularGroupTransformer.GetTranslationKey(key.Id), _dbContext);

            _module.Delete(key);
        }

        internal BodyExercise Update(BodyExercise bodyExercise)
        {
            //Update Translation Name
            //Translation.UpdateInDB(BodyExerciseTransformer.GetTranslationKey(bodyExercise.Id), bodyExercise.Name, _dbContext);

            return _module.Update(bodyExercise);
        }
    }
}
