using HybridLazyCacheLib.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
namespace HybridLazyCacheLib.Internal
{
    internal sealed class CacheEntryContext : ICacheEntryContext
    {
        public DateTimeOffset? AbsoluteExpiration { get; set; }

        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

        public TimeSpan? SlidingExpiration { get; set; }

        public CacheItemPriority Priority { get; set; } = CacheItemPriority.Normal;

        public long? Size { get; set; }

        public bool Cache { get; set; } = true;
    }
}
