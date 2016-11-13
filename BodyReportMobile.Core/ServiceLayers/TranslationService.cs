using BodyReport.Message;
using BodyReportMobile.Core.Manager;
using SQLite.Net;
using System.Collections.Generic;

namespace BodyReportMobile.Core.ServiceLayers
{
    public class TranslationService : LocalService
    {
        private const string _cacheName = "TranslationsCache";
        public TranslationService(SQLiteConnection dbContext) : base(dbContext)
        {
        }

        public TranslationVal GetTranslation(TranslationValKey key)
        {
            TranslationVal translationVal = null;
            string cacheKey = key == null ? "TranslationValKey_null" : key.GetCacheKey();
            if (key != null && !TryGetCacheData(cacheKey, out translationVal, _cacheName))
            {
                translationVal = GetTranslationManager().GetTranslation(key);
                SetCacheData(_cacheName, cacheKey, translationVal);
            }
            return translationVal;
        }

        public List<TranslationVal> FindTranslation(TranslationValCriteria criteria = null)
        {
            List<TranslationVal> translationList = null;
            string cacheKey = string.Format("TranslationValCriteria_null");
            if (!TryGetCacheData(cacheKey, out translationList, _cacheName))
            {
                translationList = GetTranslationManager().FindTranslation(criteria);
                SetCacheData(_cacheName, cacheKey, translationList);
            }
            return translationList;
        }

        public TranslationVal UpdateTranslation(TranslationVal translation)
        {
            TranslationVal result = null;
            BeginTransaction();
            try
            {
                result = GetTranslationManager().UpdateTranslation(translation);
                CommitTransaction();
                //invalidate cache
                InvalidateCache(_cacheName);
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

        public List<TranslationVal> UpdateTranslationList(List<TranslationVal> translations)
        {
            List<TranslationVal> result = new List<TranslationVal>();
            BeginTransaction();
            try
            {
                if (translations != null && translations.Count > 0)
                {
                    foreach (var translation in translations)
                    {
                        result.Add(GetTranslationManager().UpdateTranslation(translation));
                    }
                    //invalidate cache
                    InvalidateCache(_cacheName);
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
