using Microsoft.Extensions.Caching.Memory;
using System;
namespace HybridLazyCacheLib.Interfaces
{
    public interface ICacheEntryContext
    {
        DateTimeOffset? AbsoluteExpiration { get; set; }

        TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

        TimeSpan? SlidingExpiration { get; set; }

        CacheItemPriority Priority { get; set; }

        long? Size { get; set; }

        bool Cache { get; set; }
    }
}