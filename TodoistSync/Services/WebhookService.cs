using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TodoistSync.Services
{
    public class WebhookRequest
    {
        public string EventName { get; set; } = string.Empty;

        public JObject EventData { get; set; } = null!;
    }

    public interface IWebhookService
    {
        Task ProcessWebhookRequest(WebhookRequest request);
    }

    public class WebhookService : IWebhookService
    {
        public WebhookService(IItemService itemService, ISectionService sectionService, ICommentsService commentsService)
        {
            ItemService = itemService;
            SectionService = sectionService;
            CommentsService = commentsService;
        }

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
                "item:completed" => ItemCompleted(item),
                "item:deleted" => ItemCompleted(item),
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
                item.LabelIds = existingItem.LabelIds;
                await ItemService.UpdateItem(item);
            }
        }

        private async Task ItemCompleted(Item item)
        {
            var commentsTask = CommentsService.GetCommentsAsync(item.ProjectId);
            var projectItemsTask = ItemService.GetItemsInProject(item.ProjectId);
            await Task.WhenAll(commentsTask, projectItemsTask);

            var templateIdComment = commentsTask.Result.SingleOrDefault(x => x.Content.Contains("TemplateId:"));

            var templateIdString = templateIdComment?.Content.Split(":").Last();
            if (templateIdString == null || !long.TryParse(templateIdString, out var templateId) || templateId == default)
            {
                return;
            }

            if (!projectItemsTask.Result.Any())
            {
                await InvokeNextTemplateSection(templateId, item);
            }
        }

        private static Section? GetNextSection(IReadOnlyCollection<Item> templateItems, IReadOnlyCollection<Section> templateSections)
        {
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
            var templateItemsTask = ItemService.GetItemsInProject(templateId);
            var templateSectionsTask = SectionService.GetSectionsAsync(templateId);

            await Task.WhenAll(templateItemsTask, templateSectionsTask);

            var nextSection = GetNextSection(templateItemsTask.Result, templateSectionsTask.Result);

            if (nextSection == null)
            {
                return;
            }

            var nextItems = templateItemsTask.Result.Where(x => x.SectionId == nextSection.Id).ToList();

            foreach (var nextItem in nextItems)
            {
                nextItem.SectionId = null;
                nextItem.ProjectId = completedItem.ProjectId;
                nextItem.DueDateTime = nextItem.Due?.Datetime;
                nextItem.DueDate = nextItem.Due?.Datetime == null ? nextItem.Due?.Date : null;
                nextItem.UniqueId = $"{completedItem.ProjectId}{nextItem.Id}".GetHashCode().ToString();
            }

            await ItemService.PostItems(nextItems);
        }
    }
}
