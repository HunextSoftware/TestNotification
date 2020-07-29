using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TestNotificationWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    //webBuilder.UseUrls("https://localhost:6001;http://localhost:6000");
                });
    }
}
