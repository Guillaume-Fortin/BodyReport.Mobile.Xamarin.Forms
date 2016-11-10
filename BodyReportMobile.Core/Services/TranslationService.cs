using BodyReport.Message;
using BodyReportMobile.Core.Manager;
using SQLite.Net;
using System.Collections.Generic;

namespace BodyReportMobile.Core.Services
{
    public class TranslationService : LocalService
    {
        TranslationManager _manager;

        public TranslationService(SQLiteConnection dbContext) : base(dbContext)
        {
            _manager = new TranslationManager(_dbContext);
        }

        public TranslationVal GetTranslation(TranslationValKey key)
        {
            return _manager.GetTranslation(key);
        }

        public List<TranslationVal> FindTranslation(TranslationValCriteria translationValCriteria = null)
        {
            return _manager.FindTranslation(translationValCriteria);
        }

        internal TranslationVal UpdateTranslation(TranslationVal translation)
        {
            return _manager.UpdateTranslation(translation);
        }

        internal List<TranslationVal> UpdateTranslationList(List<TranslationVal> translationList)
        {
            List<TranslationVal> list = new List<TranslationVal>();
            _dbContext.BeginTransaction();
            try
            {
                foreach (var translation in translationList)
                {
                    list.Add(_manager.UpdateTranslation(translation));
                }
            }
            catch
            {
                _dbContext.Rollback();
                throw;
            }
            finally
            {
                _dbContext.Commit();
            }
            
            return list;
        }
    }
}
