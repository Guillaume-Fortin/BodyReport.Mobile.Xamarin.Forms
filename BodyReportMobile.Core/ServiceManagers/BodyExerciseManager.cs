using BodyReportMobile.Core.Crud.Module;
using BodyReportMobile.Core.Crud.Transformer;
using BodyReportMobile.Core.Framework;
using Message;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.ServiceManagers
{
    public class BodyExerciseManager : ServiceManager
    {
        BodyExerciseModule _module = null;

        public BodyExerciseManager(SQLiteConnection dbContext) : base(dbContext)
        {
            _module = new BodyExerciseModule(_dbContext);
        }

        public BodyExercise GetBodyExercise(BodyExerciseKey key)
        {
            var bodyExercise = _module.Get(key);
            if (bodyExercise != null)
                bodyExercise.Name = Translation.GetInDB(BodyExerciseTransformer.GetTranslationKey(bodyExercise.Id));
            return bodyExercise;
        }

        public List<BodyExercise> FindBodyExercises(CriteriaField criteriaField = null)
        {
            var bodyExerciseList = _module.Find(criteriaField);
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

        public BodyExercise CreateBodyExercise(BodyExercise bodyExercise)
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

        internal void DeleteBodyExercise(BodyExerciseKey key)
        {
            //Update Translation Name
            //Translation.DeleteInDB(MuscularGroupTransformer.GetTranslationKey(key.Id), _dbContext);

            _module.Delete(key);
        }

        internal BodyExercise UpdateBodyExercise(BodyExercise bodyExercise)
        {
            //Update Translation Name
            //Translation.UpdateInDB(BodyExerciseTransformer.GetTranslationKey(bodyExercise.Id), bodyExercise.Name, _dbContext);

            return _module.Update(bodyExercise);
        }

        internal List<BodyExercise> UpdateBodyExerciseList(List<BodyExercise> bodyExerciseList)
        {
            List<BodyExercise> list = new List<BodyExercise>();
            foreach (var bodyExercise in bodyExerciseList)
            {
                //Translation.UpdateInDB(MuscleTransformer.GetTranslationKey(muscle.Id), muscle.Name, _dbContext);
                list.Add(_module.Update(bodyExercise));
            }
            return list;
        }
    }
}
