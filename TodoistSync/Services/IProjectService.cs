using System.Collections.Generic;
using System.Threading.Tasks;
using TodoistSync.Models;
using TodoistSync.Models.Responses;

namespace TodoistSync.Services
{
    public interface IProjectService
    {
        public Task<ProjectCollectionResponse> GetProjectAsync(int projectId);

        /// <summary>
        /// Gets all the projects from todoist
        /// </summary>
        /// <returns></returns>
        public Task<ProjectCollectionResponse> GetAllProjectsAsync();
    }
}