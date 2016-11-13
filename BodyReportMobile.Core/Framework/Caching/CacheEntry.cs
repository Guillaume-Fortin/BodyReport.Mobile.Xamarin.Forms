using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.Framework.Caching
{
    public class CacheEntry
    {
        public string Key { get; set; } = null;
        public object Value { get; set; } = null;
        public bool IsExpired { get; set; } = false;
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
        public DateTimeOffset? AbsoluteExpiration { get; set; }

        public CacheEntry(string key, object value, TimeSpan? absoluteExpirationRelativeToNow)
        {
            Key = key;
            Value = value;
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
            AbsoluteExpiration = DateTimeOffset.UtcNow + absoluteExpirationRelativeToNow;
        }

        public bool CheckForExpiredTime()
        {
            var now = DateTimeOffset.UtcNow;
            if (AbsoluteExpiration.HasValue && AbsoluteExpiration.Value <= now)
            {
                IsExpired = true;
                return IsExpired;
            }
            return IsExpired;
        }
    }
}
