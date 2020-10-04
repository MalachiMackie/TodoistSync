using System.Collections.Generic;

namespace TodoistSync.Models.Responses
{
    public class ProjectCollectionResponse : TodoistResponse
    {
        public IEnumerable<Project> Projects { get; set; } = new List<Project>();
    }
}