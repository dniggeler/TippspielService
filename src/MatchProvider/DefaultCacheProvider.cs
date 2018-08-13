using System;
using MatchProvider.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace MatchProvider
{
    public class DefaultCacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _cache;

        public DefaultCacheProvider(IMemoryCache cache)
        {
            _cache = cache;
        }

        public object Get(string key)
        {
            if (_cache.TryGetValue(key, out object value))
            {
                return value;
            }
            return null;
        }

        public void Set(string key, object data, int cacheTime)
        {
            MemoryCacheEntryOptions policy =
                new MemoryCacheEntryOptions {AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime)};

            _cache.Set(key,data, policy);
        }

        public bool IsSet(string key)
        {
            return _cache.TryGetValue(key, out object _);
        }

        public void Invalidate(string key)
        {
            _cache.Remove(key);
        }

        public void Clear()
        {
            _cache.Dispose();
        }
    }
}