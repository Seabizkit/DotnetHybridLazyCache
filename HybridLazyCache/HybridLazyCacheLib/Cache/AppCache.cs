using HybridLazyCacheLib.Cache.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
namespace HybridLazyCacheLib.Cache
{
    public sealed class AppCache : IAppCache
    {
        readonly IMemoryCache _cache;
        readonly ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();
        public AppCache(IMemoryCache cache) { _cache = cache; }
        public bool TryGetValue<T>(object key, out T value)
        {
            if (_cache.TryGetValue(key, out var o) && o is T t) 
            { 
                value = t; 
                return true; 
            }
            value = default; 
            return false;
        }
        public void Remove(object key) => _cache.Remove(key);
        public T GetOrCreate<T>(object key, Func<ICacheEntryContext, T> factory)
        {
            if (TryGetValue<T>(key, out var v)) return v;
            var sem = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            sem.Wait();
            try
            {
                if (TryGetValue<T>(key, out v)) return v;
                var c = new CacheEntryContext(); var r = factory(c);
                var opt = new MemoryCacheEntryOptions();
                if (c.AbsoluteExpiration != null) opt.AbsoluteExpiration = c.AbsoluteExpiration;
                if (c.AbsoluteExpirationRelativeToNow != null) opt.AbsoluteExpirationRelativeToNow = c.AbsoluteExpirationRelativeToNow;
                if (c.SlidingExpiration != null) opt.SlidingExpiration = c.SlidingExpiration;
                _cache.Set(key, r, opt); return r;
            }
            finally { sem.Release(); _locks.TryRemove(key, out _); }
        }
        public async Task<T> GetOrCreateAsync<T>(object key, Func<ICacheEntryContext, CancellationToken, Task<T>> factory, CancellationToken ct = default)
        {
            if (TryGetValue<T>(key, out var v)) return v;
            var sem = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            await sem.WaitAsync(ct);
            try
            {
                if (TryGetValue<T>(key, out v)) return v;
                var c = new CacheEntryContext(); var r = await factory(c, ct);
                var opt = new MemoryCacheEntryOptions();
                if (c.AbsoluteExpiration != null) opt.AbsoluteExpiration = c.AbsoluteExpiration;
                if (c.AbsoluteExpirationRelativeToNow != null) opt.AbsoluteExpirationRelativeToNow = c.AbsoluteExpirationRelativeToNow;
                if (c.SlidingExpiration != null) opt.SlidingExpiration = c.SlidingExpiration;
                _cache.Set(key, r, opt); return r;
            }
            finally { sem.Release(); _locks.TryRemove(key, out _); }
        }
    }
}
