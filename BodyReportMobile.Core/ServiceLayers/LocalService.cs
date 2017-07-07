using BodyReportMobile.Core.Data;
using BodyReportMobile.Core.Framework;
using BodyReportMobile.Core.Framework.Caching;
using BodyReportMobile.Core.Manager;

namespace BodyReportMobile.Core.ServiceLayers
{
    public class LocalService
    {
        /// <summary>
		/// DataBase context with transaction
		/// </summary>
		protected ApplicationDbContext _dbContext = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">db context</param>
        public LocalService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Manage database transaction

        private bool _isParentTransaction = true;
        protected void BeginTransaction()
        {
            _isParentTransaction = _dbContext.Database.CurrentTransaction == null;
            if (_isParentTransaction)
            {
                _dbContext.Database.BeginTransaction();
            }
        }

        protected void CommitTransaction()
        {
            if (_isParentTransaction && _dbContext.Database.CurrentTransaction != null)
                _dbContext.Database.CurrentTransaction.Commit();
        }

        protected void RollbackTransaction()
        {
            if (_isParentTransaction && _dbContext.Database.CurrentTransaction != null)
                _dbContext.Database.CurrentTransaction.Rollback();
        }

        protected void EndTransaction()
        {
            if (_isParentTransaction && _dbContext.Database.CurrentTransaction != null)
            {
                _dbContext.Database.CurrentTransaction.Dispose();
            }
        }

        #endregion

        #region cache
        private string CompleteCacheKeyWithCulture(string cacheKey, string culture = null)
        {
            if (culture == null)
                culture = Translation.CurrentLang.ToString();
            return string.Format("{0}_{1}", culture, cacheKey);
        }

        public bool TryGetCacheData<T>(string cacheKey, out T data, string cacheName)
        {
            return MemoryCache.Instance.TryGetData<T>(CompleteCacheKeyWithCulture(cacheKey), out data, cacheName);
        }

        public void SetCacheData<T>(string cacheName, string cacheKey, T data)
        {
            const int minutes = 10;
            MemoryCache.Instance.SetData<T>( CompleteCacheKeyWithCulture(cacheKey), data, minutes, cacheName);
        }

        public void InvalidateCache(string cacheName)
        {
            MemoryCache.Instance.InvalidateCache(cacheName);
        }

        public void InvalidateAllCache()
        {
            MemoryCache.Instance.InvalidateAllCache();
        }

        #endregion

        #region manager accessor

        /// <summary>
        /// TrainingDay Manager
        /// </summary>
        private TrainingDayManager _trainingDayManager = null;

        /// <summary>
        /// Body Exercise Manager
        /// </summary>
        private BodyExerciseManager _bodyExerciseManager = null;

        /// <summary>
        /// Country Manager
        /// </summary>
        private CountryManager _countryManager = null;

        /// <summary>
        /// Body Exercise Manager
        /// </summary>
        private MuscleManager _muscleManager = null;

        /// <summary>
        /// Muscular Group Manager
        /// </summary>
        private MuscularGroupManager _muscularGroupManager = null;

        /// <summary>
        /// Training Exercise Manager
        /// </summary>
        private TrainingExerciseManager _trainingExerciseManager = null;

        /// <summary>
        /// TrainingWeek Manager
        /// </summary>
        private TrainingWeekManager _trainingWeekManager = null;

        /// <summary>
        /// Translation Manager
        /// </summary>
        private TranslationManager _translationManager = null;

        /// <summary>
        /// User info Manager
        /// </summary>
        private UserInfoManager _userInfoManager = null;

        /// <summary>
        /// Need to recreate or create new manager if null or if bd context changed
        /// </summary>
        /// <typeparam name="T">Manager type</typeparam>
        /// <param name="manager">manager</param>
        /// <returns>true if a new manager must be create</returns>
        private bool NeedCreateNewManager<T>(T manager) where T : BodyReportManager
        {
            if (manager == null || manager.DbContext != _dbContext)
                return true;
            else
                return false;
        }

        public TrainingDayManager GetTrainingDayManager()
        {
            if (NeedCreateNewManager(_trainingDayManager))
                _trainingDayManager = new TrainingDayManager(_dbContext);
            return _trainingDayManager;
        }

        public BodyExerciseManager GetBodyExerciseManager()
        {
            if (NeedCreateNewManager(_bodyExerciseManager))
                _bodyExerciseManager = new BodyExerciseManager(_dbContext);
            return _bodyExerciseManager;
        }

        public CountryManager GetCountryManager()
        {
            if (NeedCreateNewManager(_countryManager))
                _countryManager = new CountryManager(_dbContext);
            return _countryManager;
        }

        public MuscleManager GetMuscleManager()
        {
            if (NeedCreateNewManager(_muscleManager))
                _muscleManager = new MuscleManager(_dbContext);
            return _muscleManager;
        }

        public MuscularGroupManager GetMuscularGroupManager()
        {
            if (NeedCreateNewManager(_muscularGroupManager))
                _muscularGroupManager = new MuscularGroupManager(_dbContext);
            return _muscularGroupManager;
        }

        public TrainingExerciseManager GetTrainingExerciseManager()
        {
            if (NeedCreateNewManager(_trainingExerciseManager))
                _trainingExerciseManager = new TrainingExerciseManager(_dbContext);
            return _trainingExerciseManager;
        }

        public TrainingWeekManager GetTrainingWeekManager()
        {
            if (NeedCreateNewManager(_trainingWeekManager))
                _trainingWeekManager = new TrainingWeekManager(_dbContext);
            return _trainingWeekManager;
        }

        public TranslationManager GetTranslationManager()
        {
            if (NeedCreateNewManager(_translationManager))
                _translationManager = new TranslationManager(_dbContext);
            return _translationManager;
        }

        public UserInfoManager GetUserInfoManager()
        {
            if (NeedCreateNewManager(_userInfoManager))
                _userInfoManager = new UserInfoManager(_dbContext);
            return _userInfoManager;
        }

        #endregion
    }
}
