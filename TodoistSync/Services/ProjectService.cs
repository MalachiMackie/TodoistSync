using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TodoistSync.Helpers;
using TodoistSync.Models;

namespace TodoistSync.Services
{
    public class ProjectService : IProjectService
    {
        private HttpClient HttpClient { get; }
        
        private TodoistConfig TodoistConfig { get; }

        private IEnumerable<ResourceType> ResourceTypes { get; } = new []{ResourceType.projects}; 

        public ProjectService(HttpClient httpClient, IOptions<TodoistConfig> todoistConfig)
        {
            TodoistConfig = todoistConfig.Value;
            HttpClient = httpClient;
        }

        public async Task<Project> GetProjectAsync(long projectId)
        {
            var content = HttpHelpers.BuildTodoistContent(TodoistConfig, ResourceTypes, new []
            {
                KeyValuePair.Create("project_id", projectId.ToString()), 
            });

            var response = await HttpClient.PostAsync("projects/get", content);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            
            var jObject = JsonConvert.DeserializeObject<JObject>(json);
            return jObject.GetValue("project").ToObject<Project>();
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            var content = HttpHelpers.BuildTodoistContent(TodoistConfig, ResourceTypes);

            var response = await HttpClient.PostAsync("sync", content);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            
            var jObject = JsonConvert.DeserializeObject<JObject>(json);
            
            return jObject.GetValue("projects").ToObject<IEnumerable<Project>>();
        }
    }
}