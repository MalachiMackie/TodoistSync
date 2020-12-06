using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
// ReSharper disable ClassNeverInstantiated.Global

namespace TodoistSync
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(ConfigureApplication)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://0.0.0.0:5000");
                    webBuilder.UseStartup<Startup>();
                });

        private static void ConfigureApplication(HostBuilderContext ctx, IConfigurationBuilder builder)
        {
            builder.AddSystemsManager("/Mackie/TodoistSync");
        }
    }
}
