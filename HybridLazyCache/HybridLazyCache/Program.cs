namespace HybridLazyCache
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("AppCache tests");
            Console.WriteLine("");
            var wrapper = new Wrapper();


            await wrapper.ParelleHitsAsync();
            await wrapper.AbsoluteExpirationTest();
            await wrapper.RemoveTest();
            await wrapper.SlidingExpirationTest();
            

            Console.WriteLine("Success - Press ENTER");
            Console.ReadLine();
        }
    }
}
