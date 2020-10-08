using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
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
        private ITemplateService TemplateService { get; }

        public ProjectsController(IProjectService projectService, ITemplateService templateService)
        {
            ProjectService = projectService;
            TemplateService = templateService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> Get()
        {
            return Ok(await ProjectService.GetAllProjectsAsync());
        }

        [HttpGet("{projectId}")]
        public async Task<ActionResult<Project>> Get(long projectId)
        {
            return Ok(await ProjectService.GetProjectAsync(projectId));
        }

        [HttpGet("{projectId}/as-template")]
        public async Task<ActionResult<string>> GetProjectAsTemplate(long projectId)
        {
            return Ok(await TemplateService.GetProjectAsTemplateCSV(projectId));
        }
    }
}