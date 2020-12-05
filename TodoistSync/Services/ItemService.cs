using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TodoistSync.Services
{

    public class ItemDue
    {
        public DateTime? Datetime { get; set; }

        public string Date { get; set; } = string.Empty;
    }

    public class Item
    {
        public long Id { get; set; }

        public long ProjectId { get; set; }

        public string Content { get; set; } = string.Empty;

        public long? SectionId { get; set; }

        public ItemDue? Due { get; set; }

        public DateTime? DueDateTime { get; set; }

        public string? DueDate { get; set; }

        public long Order { get; set; }

        public long ParentId { get; set; }

        public IEnumerable<long>? LabelIds { get; set; }
    }

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
