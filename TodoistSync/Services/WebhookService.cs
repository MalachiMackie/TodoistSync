using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TodoistSync.Services
{
    public record WebhookRequest(string EventName, JObject EventData);

    public interface IWebhookService
    {
        Task ProcessWebhookRequest(WebhookRequest request);
    }

    public class WebhookService : IWebhookService
    {
        public WebhookService(IItemService itemService, IOptions<TodoistConfig> todoistConfig, ISectionService sectionService, ICommentsService commentsService)
        {
            ItemService = itemService;
            SectionService = sectionService;
            CommentsService = commentsService;
            TodoistConfig = todoistConfig.Value;
        }

        private TodoistConfig TodoistConfig { get; }

        private IItemService ItemService { get; }

        private ISectionService SectionService { get; }

        private ICommentsService CommentsService { get; }

        public async Task ProcessWebhookRequest(WebhookRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var item = JsonConvert.DeserializeObject<Item>(request.EventData.ToString()) ??
                       throw new ArgumentNullException(nameof(request));

            var task = request.EventName switch
            {
                "item:completed" or "item:deleted" => ItemCompleted(item),
                "item:added" => ItemAdded(item), // add labels of current tasks to new task
                _ => throw new InvalidOperationException($"{request.EventName} is not an accepted event name.")
            };

            await task;
        }

        private async Task ItemAdded(Item item)
        {
            var items = await ItemService.GetItemsInProject(item.ProjectId);
            var existingItem = items.FirstOrDefault(x => x.Id != item.Id);
            if (existingItem != null)
            {
                await ItemService.UpdateItem(item with{LabelIds = existingItem.LabelIds});
            }
        }

        private async Task ItemCompleted(Item item)
        {
            var comments = await CommentsService.GetCommentsAsync(item.ProjectId);
            var templateId =
                TodoistConfig.TemplateIds.SingleOrDefault(x => comments.Any(y => y.Content == $"TemplateId:{x}"));
            if (templateId == default)
            {
                return;
            }

            var items = await ItemService.GetItemsInProject(item.ProjectId);
            if (!items.Any())
            {
                await InvokeNextTemplateSection(templateId, item);
            }
        }

        private async Task<Section?> GetNextSection(long projectId, Item completedItem, IReadOnlyCollection<Item> templateItems)
        {
            var templateSections = await SectionService.GetSectionsAsync(projectId);
            using var enumerator = templateSections.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = templateItems.First(x => x.SectionId == enumerator.Current.Id);
                if (item.LabelIds != null
                    && item.LabelIds.Any()
                    && enumerator.MoveNext())
                {
                    return enumerator.Current;
                }
            }

            return null;
        }

        private async Task InvokeNextTemplateSection(long templateId, Item completedItem)
        {
            var allItems = await ItemService.GetItemsInProject(templateId);

            var nextSection = await GetNextSection(templateId, completedItem, allItems);

            if (nextSection == null)
            {
                return;
            }

            var nextItems = allItems.Where(x => x.SectionId == nextSection.Id);

            var newItems = nextItems.Select(x => x with {SectionId = null, ProjectId = completedItem.ProjectId, DueDatetime =
                x.Due?.Datetime, DueDate =
                (x.Due?.Datetime == null) ? x.Due?.Date : null}).ToList();
            await ItemService.PostItems(newItems);
        }
    }
}
