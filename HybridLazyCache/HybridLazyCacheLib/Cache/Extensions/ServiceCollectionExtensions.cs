using HybridLazyCacheLib.Cache.Interfaces;
using Microsoft.Extensions.DependencyInjection;
namespace HybridLazyCacheLib.Cache.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHybridLazyCache(this IServiceCollection s)
        {
            s.AddMemoryCache(); 
            s.AddSingleton<IAppCache, AppCache>(); 
            return s;
        }
    }
}
