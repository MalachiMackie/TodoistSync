using System.Collections.Generic;

namespace TodoistSync
{
    public class TodoistConfig
    {
        public IReadOnlyCollection<long> TemplateIds { get; set; } = new List<long>();
    }
}
