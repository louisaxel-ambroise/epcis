using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FasTnT.Host
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder
                    .UseStartup<Startup>()
                    .ConfigureKestrel(s => s.AddServerHeader = false));
    }
}
