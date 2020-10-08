using System.Collections.Generic;
using System.Threading.Tasks;
using TodoistSync.Models;

namespace TodoistSync.Services
{
    public interface IProjectService
    {
        public Task<Project> GetProjectAsync(long projectId);

        /// <summary>
        /// Gets all the projects from todoist
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Project>> GetAllProjectsAsync();
    }
}