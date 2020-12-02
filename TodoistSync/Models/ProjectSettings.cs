using System.ComponentModel.DataAnnotations;

namespace TodoistSync.Models
{
    public class ProjectSettings
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public long? ParentId { get; set; }

        public int? Color { get; set; }

        public long? FromTemplateId { get; set; }
    }
}
