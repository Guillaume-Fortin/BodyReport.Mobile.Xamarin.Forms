using BodyReport.Message;
using BodyReportMobile.Core.Manager;
using SQLite.Net;
using System.Collections.Generic;

namespace BodyReportMobile.Core.ServiceLayers
{
    public class TranslationService : LocalService
    {
        public TranslationService(SQLiteConnection dbContext) : base(dbContext)
        {
        }

        public TranslationVal GetTranslation(TranslationValKey key)
        {
            return GetTranslationManager().GetTranslation(key);
        }

        public List<TranslationVal> FindTranslation(TranslationValCriteria translationValCriteria = null)
        {
            return GetTranslationManager().FindTranslation(translationValCriteria);
        }

        public TranslationVal UpdateTranslation(TranslationVal translation)
        {
            TranslationVal result = null;
            BeginTransaction();
            try
            {
                result = GetTranslationManager().UpdateTranslation(translation);
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                EndTransaction();
            }

            return result;
        }

        public List<TranslationVal> UpdateTranslationList(List<TranslationVal> translationList)
        {
            List<TranslationVal> result = new List<TranslationVal>();
            BeginTransaction();
            try
            {
                foreach (var translation in translationList)
                {
                    result.Add(GetTranslationManager().UpdateTranslation(translation));
                }
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                EndTransaction();
            }
            
            return result;
        }
    }
}
