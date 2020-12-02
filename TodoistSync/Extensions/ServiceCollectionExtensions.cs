using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TodoistSync.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTodoistHttpClient<TClient, TImplementation>(
            this IServiceCollection serviceCollection, IConfiguration configuration)
            where TClient : class
            where TImplementation : class, TClient
        {
            serviceCollection.AddHttpClient<TClient, TImplementation>(client =>
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration.GetValue<string>("ApiKey")}");
                client.BaseAddress = new Uri("https://api.todoist.com/rest/v1/");
            });

            return serviceCollection;
        }
    }
}
