using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using HybridLazyCacheLib.Interfaces;

namespace HybridLazyCacheLib
{
    public sealed class AppCache : IHybridLazyCache
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentDictionary<object, SemaphoreSlim> _locks 
            = new ConcurrentDictionary<object, SemaphoreSlim>();

        public AppCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public bool TryGetValue<T>(object key, out T value)
        {
            return _cache.TryGetValue(key, out value);
        }

        public void Remove(object key)
        {
            _cache.Remove(key);
        }

        public void Clear()
        {
            if (_cache is MemoryCache mc)
                mc.Compact(1.0);
        }

        public T GetOrCreate<T>(object key, Func<ICacheEntryContext, T> factory)
        {
            if (TryGetValue(key, out T value))
                return value;
            var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            semaphore.Wait();
            try
            {
                if (TryGetValue(key, out value))
                    return value;
                var context = new CacheEntryContext();
                value = factory(context);
                if (context.Cache)
                {
                    _cache.Set( key,  value, BuildOptions(context));
                }
                return value;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<T> GetOrCreateAsync<T>(object key, Func<ICacheEntryContext, CancellationToken, Task<T>> factory,
            CancellationToken cancellationToken = default)
        {
            if (TryGetValue(key, out T value))
                return value;
            var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                if (TryGetValue(key, out value))
                    return value;
                var context = new CacheEntryContext();
                value = await factory(context, cancellationToken);
                if (context.Cache)
                {
                    _cache.Set(key, value, BuildOptions(context));
                }
                return value;
            }
            finally
            {
                semaphore.Release();
            }
        }

        private static MemoryCacheEntryOptions BuildOptions(CacheEntryContext context)
        {
            var options = new MemoryCacheEntryOptions
            {
                Priority = context.Priority
            };

            if (context.AbsoluteExpiration.HasValue)
                options.AbsoluteExpiration = context.AbsoluteExpiration;

            if (context.AbsoluteExpirationRelativeToNow.HasValue)
                options.AbsoluteExpirationRelativeToNow =
                    context.AbsoluteExpirationRelativeToNow;

            if (context.SlidingExpiration.HasValue)
                options.SlidingExpiration =
                    context.SlidingExpiration;

            if (context.Size.HasValue)
                options.Size = context.Size.Value;

            return options;
        }
    }

}
