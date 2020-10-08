using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using TodoistSync.Models;

namespace TodoistSync.Helpers
{
    public static class HttpHelpers
    {
        public static HttpContent BuildTodoistContent(
            TodoistConfig config, IEnumerable<ResourceType> resourceTypes)
            => BuildTodoistContent(config, resourceTypes, new KeyValuePair<string, string>[] { });
        
        public static HttpContent BuildTodoistContent(
            TodoistConfig config,
            IEnumerable<ResourceType>? resourceTypes = null,
            IEnumerable<KeyValuePair<string, string>>? contentValues = null)
        {
            var values = new Dictionary<string, string>(contentValues ?? new KeyValuePair<string, string>[]{});
            values.TryAdd("token", config.ApiKey);
            values.TryAdd("sync_token", "*");
            if (resourceTypes != null)
            {
                values.TryAdd("resource_types", $"[{string.Join(",", resourceTypes.Select(x => $"\"{x}\""))}]");   
            }
            var content = new FormUrlEncodedContent(values);
            return content;
        }
    }
}