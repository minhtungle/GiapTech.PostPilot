using Infrastructure.Enums;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace Infrastructure.Caching
{
    public class MemoryCacheManager : ICacheManager
    {
        private ObjectCache Cache => MemoryCache.Default;

        public T Get<T>(string key)
        {
            return (T)Cache[key];
        }

        public T Get<T>(string key, Func<T> acquire, int cacheMinutes = 30)
        {
            if (IsSet(key))
                return Get<T>(key);

            var result = acquire();
            Set(key, result, cacheMinutes);
            return result;
        }

        public void Set<T>(string key, T data, int cacheMinutes = 30)
        {
            if (data == null) return;

            var policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheMinutes) };
            Cache.Set(key, data, policy);
        }
        public void Set<T>(string key, T data, CacheDuration duration)
        {
            if (data == null) return;

            CacheItemPolicy policy;

            if ((int)duration == -1) // NeverExpire
            {
                policy = new CacheItemPolicy { AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration };
            }
            else if ((int)duration > 0)
            {
                policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes((int)duration) };
            }
            else
            {
                throw new ArgumentException("Dùng giá trị hợp lệ hoặc gọi hàm Set với customMinutes");
            }

            Cache.Set(key, data, policy);
        }
        public bool IsSet(string key)
        {
            return Cache.Contains(key);
        }
        public void Remove(string key)
        {
            Cache.Remove(key);
        }
    }

}