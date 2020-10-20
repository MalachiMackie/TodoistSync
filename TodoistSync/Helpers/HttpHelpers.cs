using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using TodoistSync.Models;

namespace TodoistSync.Helpers
{
    
    public static class HttpHelpers
    {   
        public static HttpContent BuildTodoistContent(
            TodoistConfig config,
            IEnumerable<ResourceType>? resourceTypes = null,
            IEnumerable<KeyValuePair<string, string>>? contentValues = null)
        {
            var values = contentValues != null
                ? new Dictionary<string, string>(contentValues)
                : new Dictionary<string, string>();
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