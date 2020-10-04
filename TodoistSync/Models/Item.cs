using System.Collections.Generic;

namespace TodoistSync.Models
{
    public class Item
    {
        public int Id { get; set; }

        public string Content { get; set; } = string.Empty;
        
        public IEnumerable<int> Labels { get; set; } = new List<int>();
    }
}