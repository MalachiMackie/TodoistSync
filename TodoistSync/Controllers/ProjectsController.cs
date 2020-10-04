using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoistSync.Models;
using TodoistSync.Services;

namespace TodoistSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private IProjectService ProjectService { get; }

        public ProjectsController(IProjectService projectService)
        {
            ProjectService = projectService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> Get()
        {
            return Ok((await ProjectService.GetAllProjectsAsync()).Projects);
        }
    }
}