using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TodoistSync.Helpers;

namespace TodoistSync.Services
{
    public class TemplateService : ITemplateService
    {
        private HttpClient Client { get; }

        private TodoistConfig TodoistConfig { get; }

        public TemplateService(HttpClient client, IOptions<TodoistConfig> todoistConfig)
        {
            Client = client;
            TodoistConfig = todoistConfig.Value;
        }
        
        public async Task<string> GetProjectAsTemplateCSV(long projectId)
        {
            var content = HttpHelpers.BuildTodoistContent(TodoistConfig, contentValues: new[]
            {
                new KeyValuePair<string, string>("project_id", projectId.ToString())
            });
            var response = await Client.PostAsync("templates/export_as_file", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}