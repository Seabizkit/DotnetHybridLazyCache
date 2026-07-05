using HybridLazyCacheLib;
using HybridLazyCacheLib.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using HybridLazyCacheLib.Extensions;
namespace HybridLazyCache
{
    public class Wrapper 
    {

        IHybridLazyCache _cache;
      
        public Wrapper()
        { 
            _cache = WithDI();
        }

        private IHybridLazyCache WithDI()
        {
            var services = new ServiceCollection();
            services.AddHybridLazyCache();
            var provider = services.BuildServiceProvider();
            var cache = provider.GetRequiredService<IHybridLazyCache>();
            return cache;
        }

        private IHybridLazyCache WithoutDI()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            IHybridLazyCache cache = new AppCache(memoryCache);
            return cache;
        }

        

        public  async Task ParelleHitsAsync()
        {
            Console.WriteLine("ParelleHits");
            LineBeginBreak();
            var tasks = new List<Task>();
            for (int i = 0; i < 1000; i++)
                tasks.Add(GetCacheItem());
            await Task.WhenAll(tasks);
            LineEndBreak();
        }

        private static void LineBeginBreak()
        {
            Console.WriteLine("...................BEGIN.........................");
        }
        private static void LineEndBreak()
        {
            Console.WriteLine("....................END..........................");
        }

        public async Task SlidingExpirationTest()
        {
            Console.WriteLine("SlidingExpirationTest...");
            LineBeginBreak();
            Guid previous = Guid.Empty;

            for (int i = 1; i <= 5; i++)
            {
                var value = await _cache.GetOrCreateAsync(
                    "slide",
                    async (entry, ct) =>
                    {
                        Console.WriteLine("Factory");

                        entry.SlidingExpiration =
                            TimeSpan.FromSeconds(2);

                        return Guid.NewGuid();
                    });

                Console.WriteLine($"{i}: {value}");

                if (previous != Guid.Empty)
                    Console.WriteLine(previous == value);

                previous = value;

                await Task.Delay(1000);
            }

            Console.WriteLine("Waiting for expiration...");

            await Task.Delay(3000);

            var last = await _cache.GetOrCreateAsync(
                "slide",
                async (entry, ct) =>
                {
                    Console.WriteLine("Factory");

                    entry.SlidingExpiration =
                        TimeSpan.FromSeconds(2);

                    return Guid.NewGuid();
                });

            Console.WriteLine(last);
            LineEndBreak();
        }

        public async Task RemoveTest()
        {
            Console.WriteLine("RemoveTest...");
            LineBeginBreak();
            var value1 = await _cache.GetOrCreateAsync(
                "remove",
                async (entry, ct) =>
                {
                    Console.WriteLine("Factory");

                    return Guid.NewGuid();
                });

            _cache.Remove("remove");

            var value2 = await _cache.GetOrCreateAsync(
                "remove",
                async (entry, ct) =>
                {
                    Console.WriteLine("Factory");

                    return Guid.NewGuid();
                });

            Console.WriteLine(value1 == value2);
            LineEndBreak();
        }

        public async Task AbsoluteExpirationTest()
        {
            Console.WriteLine("RemoveTest...");
            LineBeginBreak();

            var value1 = await _cache.GetOrCreateAsync(
                "expire",
                async (entry, ct) =>
                {
                    Console.WriteLine("Factory");

                    entry.AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromSeconds(2);

                    return Guid.NewGuid();
                });

            await Task.Delay(1000);

            var value2 = await _cache.GetOrCreateAsync(
                "expire",
                async (entry, ct) =>
                {
                    Console.WriteLine("Factory");

                    return Guid.NewGuid();
                });

            Console.WriteLine(value1 == value2);

            await Task.Delay(2500);

            var value3 = await _cache.GetOrCreateAsync(
                "expire",
                async (entry, ct) =>
                {
                    Console.WriteLine("Factory");

                    return Guid.NewGuid();
                });

            Console.WriteLine(value1 == value3);

            LineEndBreak();
        }

        public  async Task GetCacheItem()
        {
            var key = "test";
            var value = await _cache.GetOrCreateAsync(key, async (entry, ct) =>
            {
                Console.WriteLine("Factory called");
                entry.SlidingExpiration = TimeSpan.FromSeconds(30);
                await Task.Delay(1000, ct);
                return DateTime.Now.ToString();
            });
        }
    }
}
