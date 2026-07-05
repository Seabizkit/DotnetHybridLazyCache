using HybridLazyCacheLib.Cache.Interfaces;
using System;
namespace HybridLazyCacheLib.Cache
{
    sealed class CacheEntryContext : ICacheEntryContext
    {
        public DateTimeOffset? AbsoluteExpiration { get; set; }
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }
    }
}
