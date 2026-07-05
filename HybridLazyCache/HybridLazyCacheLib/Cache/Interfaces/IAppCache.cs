using System;
using System.Threading;
using System.Threading.Tasks;
namespace HybridLazyCacheLib.Cache.Interfaces
{
    public interface IAppCache
    {
        Task<T> GetOrCreateAsync<T>(object key, Func<ICacheEntryContext,CancellationToken,Task<T>> factory, CancellationToken ct=default);
        T GetOrCreate<T>(object key,Func<ICacheEntryContext,T> factory);
        void Remove(object key);
        bool TryGetValue<T>(object key,out T value);
    }
}