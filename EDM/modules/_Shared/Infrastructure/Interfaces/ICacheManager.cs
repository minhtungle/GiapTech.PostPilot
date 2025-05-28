using Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infrastructure.Interfaces
{
    public interface ICacheManager
    {
        T Get<T>(string key, Func<T> acquire, int cacheMinutes = 30);
        T Get<T>(string key);
        void Set<T>(string key, T data, CacheDuration duration);
        void Set<T>(string key, T data, int customMinutes);
        bool IsSet(string key);
        void Remove(string key);
    }

}