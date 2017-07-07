using System;
using BodyReportMobile.Core.Crud.Module;
using BodyReport.Message;
using System.Collections.Generic;
using BodyReportMobile.Core.Data;

namespace BodyReportMobile.Core.Manager
{
	public class TranslationManager : BodyReportManager
    {
		TranslationModule _module = null;

		public TranslationManager(ApplicationDbContext dbContext) : base(dbContext)
		{
			_module = new TranslationModule(DbContext);
		}

		internal TranslationVal GetTranslation(TranslationValKey key)
		{
			return _module.Get(key);
		}

		public List<TranslationVal> FindTranslation(TranslationValCriteria translationValCriteria = null)
		{
			return _module.Find(translationValCriteria);
		}

		internal TranslationVal UpdateTranslation(TranslationVal translation)
		{
			return _module.Update(translation);
		}
	}
}

