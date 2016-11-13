using System;
using BodyReportMobile.Core.Crud.Module;
using System.Collections.Generic;
using BodyReport.Message;
using BodyReportMobile.Core.Crud.Transformer;
using SQLite.Net;
using BodyReportMobile.Core.Framework;

namespace BodyReportMobile.Core.Manager
{
	public class MuscleManager : BodyReportManager
    {
		MuscleModule _module;
		public MuscleManager(SQLiteConnection dbContext) : base(dbContext)
		{
			_module = new MuscleModule(DbContext);
		}

		public List<Muscle> FindMuscles(MuscleCriteria muscleCriteria = null)
		{
			var muscleList = _module.Find(muscleCriteria);
            if (muscleList != null)
            {
                foreach(var muscle in muscleList)
                {
                    if(muscle != null)
                        muscle.Name = Translation.GetInDB(MuscleTransformer.GetTranslationKey(muscle.Id));
                }
            }

            return muscleList;
        }

		internal Muscle GetMuscle(MuscleKey key)
		{
			var muscle = _module.Get(key);
            if(muscle != null)
                muscle.Name = Translation.GetInDB(MuscleTransformer.GetTranslationKey(muscle.Id));
            return muscle;
        }

        internal Muscle Update(Muscle muscle)
		{
			//Update Translation Name
			//Translation.UpdateInDB(MuscleTransformer.GetTranslationKey(muscle.Id), muscle.Name, _dbContext);

			return _module.Update(muscle);
		}
	}
}

