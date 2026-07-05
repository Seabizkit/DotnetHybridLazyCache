using System;
namespace HybridLazyCacheLib.Cache.Interfaces
{
    public interface ICacheEntryContext
    {
        DateTimeOffset? AbsoluteExpiration{get;set;}
        TimeSpan? AbsoluteExpirationRelativeToNow{get;set;}
        TimeSpan? SlidingExpiration{get;set;}
    }
}