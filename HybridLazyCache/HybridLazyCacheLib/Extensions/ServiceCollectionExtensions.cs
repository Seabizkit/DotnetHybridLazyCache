using HybridLazyCacheLib.Interfaces;
using Microsoft.Extensions.DependencyInjection;
namespace HybridLazyCacheLib.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHybridLazyCache(this IServiceCollection s)
        {
            s.AddMemoryCache(); 
            s.AddSingleton<IHybridLazyCache, AppCache>(); 
            return s;
        }
    }
}
