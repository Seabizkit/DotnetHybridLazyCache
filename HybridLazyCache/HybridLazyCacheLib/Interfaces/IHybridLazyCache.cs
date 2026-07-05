using System;
using System.Threading;
using System.Threading.Tasks;
namespace HybridLazyCacheLib.Interfaces
{
    public interface IHybridLazyCache
    {
        bool TryGetValue<T>(object key, out T value);
        T GetOrCreate<T>( object key, Func<ICacheEntryContext, T> factory);
        Task<T> GetOrCreateAsync<T>(  object key, Func<ICacheEntryContext, CancellationToken, Task<T>> factory, CancellationToken ct = default);
        void Remove(object key);
        void Clear();
    }
}