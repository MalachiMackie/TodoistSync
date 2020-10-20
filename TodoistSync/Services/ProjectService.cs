using System;
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

        public async Task<long> CreateProject(ProjectSettings projectSettings)
        {
            string tempId = Guid.NewGuid().ToString();
            string uuid = Guid.NewGuid().ToString();
            var commands = new[]
            {
                new
                {
                    uuid,
                    temp_id = tempId,
                    type = "project_add",
                    args = new
                    {
                        name = projectSettings.Name,
                        color = projectSettings.Color,
                        parent_id = projectSettings.ParentId
                    }
                }
            };
            
            var content = HttpHelpers.BuildTodoistContent(TodoistConfig, contentValues: new[]
            {
                new KeyValuePair<string, string>("commands", JsonConvert.SerializeObject(commands)), 
            });

            var response = await HttpClient.PostAsync("sync", content);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            JObject jObject = JsonConvert.DeserializeObject<JObject>(json);
            var syncStatus = jObject.GetValue("sync_status")[uuid].Value<string>();

            if (syncStatus == "ok")
            {
                return jObject.GetValue("temp_id_mapping")[tempId].Value<long>();
            }
            throw new InvalidOperationException($"Something went wrong. Status {uuid} : {syncStatus}. Temp Id {tempId}");
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