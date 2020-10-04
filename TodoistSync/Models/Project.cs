using System.Collections.Generic;

namespace TodoistSync.Models
{
    public class Project
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;
        
        public long? ParentId { get; set; }
        
        public IEnumerable<Section> Sections { get; set; } = new List<Section>();
        
        public IEnumerable<Item> Items { get; set; } = new List<Item>();
    }
}