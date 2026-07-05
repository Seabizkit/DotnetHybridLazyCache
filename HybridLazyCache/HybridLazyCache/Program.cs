namespace HybridLazyCache
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("AppCache tests");
            var wrapper = new Wrapper();
            //await wrapper.TestAsync();

            await wrapper.RemoveTest();

            Console.WriteLine("Success - Press ENTER");
            Console.ReadLine();
        }
    }
}
