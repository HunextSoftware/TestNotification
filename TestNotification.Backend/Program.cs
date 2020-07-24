using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
namespace TestNotificationBackend
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
                    // Use it if the service is loaded on Azure
                    webBuilder.UseStartup<Startup>();

                    // RECOMMENDED FOR DEBUG --> only when the service is localhost
                    //webBuilder.UseStartup<Startup>();
                    //webBuilder.UseKestrel();
                    //webBuilder.UseUrls("http://localhost:5000", "http://172.16.1.180:5000");
                    //webBuilder.UseIISIntegration();
                });
    }
}
