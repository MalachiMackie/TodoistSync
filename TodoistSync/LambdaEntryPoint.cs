using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace TodoistSync
{
    public class LambdaEntryPoint : Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction
    {
        protected override void Init(IHostBuilder builder)
        {
            builder.ConfigureWebHostDefaults(webHostBuilder => webHostBuilder.UseStartup<Startup>())
                .ConfigureAppConfiguration(ConfigureApplication);
        }

        private static void ConfigureApplication(HostBuilderContext ctx, IConfigurationBuilder builder)
        {
            builder.AddSystemsManager("/Mackie/TodoistSync");
        }
    }
}
