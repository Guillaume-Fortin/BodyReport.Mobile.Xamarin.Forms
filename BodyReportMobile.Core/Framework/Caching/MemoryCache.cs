using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Framework.Caching
{
    public class MemoryCache
    {
        DateTimeOffset? _lastPurgeTime = null;
        private static MemoryCache _instance = null;
        public static MemoryCache Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new MemoryCache();
                }
                return _instance;
            }
        }

        private const string DefaultCacheName = "default";
        private Dictionary<string, List<CacheEntry>> _cacheDatas = new Dictionary<string, List<CacheEntry>>();

        private MemoryCache()
        {

        }
        public void SetData<T>(string key, T value, int minutes, string cacheName = DefaultCacheName)
        {
            lock(_cacheDatas)
            {
                //Get by cachename
                List<CacheEntry> cacheEntryList;
                if (!_cacheDatas.TryGetValue(cacheName, out cacheEntryList))
                {
                    cacheEntryList = new List<CacheEntry>();
                    _cacheDatas.Add(cacheName, cacheEntryList);
                }
                cacheEntryList.RemoveAll(e => e.Key == key);
                cacheEntryList.Add(new CacheEntry(key, value, TimeSpan.FromMinutes(minutes)));
            }
        }

        public void InvalidateAllCache()
        {
            lock (_cacheDatas)
            {
                _cacheDatas.Clear();
            }
        }

        public void InvalidateCache(string cacheName = DefaultCacheName)
        {
            lock (_cacheDatas)
            {
                List<CacheEntry> cacheEntryList;
                if (_cacheDatas.TryGetValue(cacheName, out cacheEntryList))
                {
                    cacheEntryList.Clear();
                }
            }
        }

        public void RemoveByKey(string key, string cacheName = DefaultCacheName)
        {
            lock (_cacheDatas)
            {
                List<CacheEntry> cacheEntryList;
                if (_cacheDatas.TryGetValue(cacheName, out cacheEntryList))
                {
                    cacheEntryList.RemoveAll(e => e.Key == key);
                }
            }
        }

        public bool TryGetData<T>(string key, out T value, string cacheName = DefaultCacheName)
        {
            bool result = false;
            value = default(T);
            lock (_cacheDatas)
            {
                PurgeExpiredCache();
                List<CacheEntry> cacheEntryList;
                if (_cacheDatas.TryGetValue(cacheName, out cacheEntryList))
                {
                    var entry = cacheEntryList.Where(e => e.Key == key).FirstOrDefault();
                    if(entry != null)
                    {
                        if (entry.CheckForExpiredTime())
                        {
                            cacheEntryList.Remove(entry);
                        }
                        else
                        {
                            result = true;
                            value = (T)entry.Value;
                        }
                    }
                }
            }
            return result;
        }

        private void PurgeExpiredCache()
        {
            lock (_cacheDatas)
            {
                if (!_lastPurgeTime.HasValue || _lastPurgeTime.Value <= DateTimeOffset.UtcNow)
                {
                    foreach (var cacheKeyVal in _cacheDatas)
                    {
                        foreach (var entry in cacheKeyVal.Value)
                        {
                            entry.CheckForExpiredTime();
                        }
                        cacheKeyVal.Value.RemoveAll(e => e.IsExpired);
                    }
                    _lastPurgeTime = DateTimeOffset.UtcNow + TimeSpan.FromMinutes(1); // only max all 1 minute
                }
            }
        }
    }
}
