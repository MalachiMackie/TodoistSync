using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TodoistSync.Services
{

    public record ItemDue(DateTime? Datetime, string Date);

    public record Item(long Id, long ProjectId, string Content, long? SectionId, ItemDue? Due, DateTime? DueDatetime, string? DueDate, long Order, long ParentId, IEnumerable<long>? LabelIds);

    public interface IItemService
    {
        public Task<IReadOnlyCollection<Item>> GetItemsInProject(long projectId);

        public Task UpdateItem(Item item);

        public Task PostItems(IReadOnlyCollection<Item> items);
    }

    public class ItemService : IItemService
    {
        private HttpClient HttpClient { get; }

        public ItemService(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<Item>> GetItemsInProject(long projectId)
        {
            var response = await HttpClient.GetAsync($"tasks?project_id={projectId}");
            return JsonConvert.DeserializeObject<List<Item>>(await response.Content.ReadAsStringAsync());
        }

        public async Task UpdateItem(Item item)
        {
            var content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync($"tasks/{item.Id}", content);

            response.EnsureSuccessStatusCode();
        }

        public async Task PostItems(IReadOnlyCollection<Item> items)
        {
            var responses = new List<HttpResponseMessage>();
            foreach (var item in items)
            {
                var content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
                var response = await HttpClient.PostAsync("tasks", content);
                responses.Add(response);
            }

            responses.ForEach(r => r.EnsureSuccessStatusCode());
        }
    }
}
