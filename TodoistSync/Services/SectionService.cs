using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TodoistSync.Services
{
    public record Section(long Id, string Name);

    public interface ISectionService
    {
        Task<IReadOnlyCollection<Section>> GetSectionsAsync(long? projectId = null);
    }

    public class SectionService : ISectionService
    {
        private HttpClient HttpClient { get; }

        public SectionService(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<Section>> GetSectionsAsync(long? projectId = null)
        {
            string url = "sections";
            if (projectId.HasValue) url += $"?project_id={projectId}";
            var response = await HttpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<List<Section>>(await response.Content.ReadAsStringAsync());
        }
    }
}
