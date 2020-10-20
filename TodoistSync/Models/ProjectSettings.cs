using System.ComponentModel.DataAnnotations;

namespace TodoistSync.Models
{
    public class ProjectSettings
    {
        [Required]
        public string Name { get; set; } = null!;

        public long? ParentId { get; set; }
        
        public int? Color { get; set; }
    }
}