using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TodoistSync.Models.Responses;

namespace TodoistSync.Services
{
    public class ProjectService : IProjectService
    {
        private HttpClient HttpClient { get; }
        
        private TodoistConfig TodoistConfig { get; }

        private string[] ResourceTypes { get; } = {"projects"}; 

        public ProjectService(HttpClient httpClient, IOptions<TodoistConfig> todoistConfig)
        {
            TodoistConfig = todoistConfig.Value;
            HttpClient = httpClient;
        }

        public async Task<ProjectCollectionResponse> GetProjectAsync(int projectId)
        {
            var pairs = new[]
            {
                new KeyValuePair<string, string>("resource_types", JsonConvert.SerializeObject(ResourceTypes)),
                new KeyValuePair<string, string>("sync_token", "*"),
                new KeyValuePair<string, string>("token", TodoistConfig.ApiKey)
            };
            var content = new FormUrlEncodedContent(pairs);

            var response = await HttpClient.PostAsync("https://api.todoist.com/sync/v8/sync", content);

            var projects = JsonConvert.DeserializeObject<ProjectCollectionResponse>(
                await response.Content.ReadAsStringAsync());

            return projects;
        }

        public async Task<ProjectCollectionResponse> GetAllProjectsAsync()
        {
            var pairs = new[]
            {
                new KeyValuePair<string, string>("resource_types", JsonConvert.SerializeObject(ResourceTypes)),
                new KeyValuePair<string, string>("sync_token", "*"),
                new KeyValuePair<string, string>("token", TodoistConfig.ApiKey)
            };
            var content = new FormUrlEncodedContent(pairs);

            var response = await HttpClient.PostAsync("https://api.todoist.com/sync/v8/sync", content);

            var projects = JsonConvert.DeserializeObject<ProjectCollectionResponse>(
                await response.Content.ReadAsStringAsync());

            return projects;
        }
    }
}