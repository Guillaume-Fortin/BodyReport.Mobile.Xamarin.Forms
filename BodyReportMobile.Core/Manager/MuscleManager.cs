using System;
using BodyReportMobile.Core.Crud.Module;
using System.Collections.Generic;
using Message;
using BodyReportMobile.Core.Crud.Transformer;
using SQLite.Net;

namespace BodyReportMobile.Core.Manager
{
	public class MuscleManager : ServiceManager
	{
		MuscleModule _module;
		public MuscleManager(SQLiteConnection dbContext) : base(dbContext)
		{
			_module = new MuscleModule(_dbContext);
		}

		public List<Muscle> FindMuscles(MuscleCriteria criteria = null)
		{
			return _module.Find(criteria);
		}

		internal Muscle GetMuscle(MuscleKey key)
		{
			return _module.Get(key);
		}

		internal Muscle UpdateMuscle(Muscle muscle)
		{
			//Update Translation Name
			//Translation.UpdateInDB(MuscleTransformer.GetTranslationKey(muscle.Id), muscle.Name, _dbContext);

			return _module.Update(muscle);
		}

		internal List<Muscle> UpdateMuscleList(List<Muscle> muscleList)
		{
			List<Muscle> list = new List<Muscle> ();
			foreach (var muscle in muscleList)
			{
				//Translation.UpdateInDB(MuscleTransformer.GetTranslationKey(muscle.Id), muscle.Name, _dbContext);
				list.Add(_module.Update (muscle));
			}
			return list;
		}
	}
}

