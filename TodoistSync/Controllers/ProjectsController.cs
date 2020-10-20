using System.Collections.Generic;
using System.IO;
using System.Text;
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

        [HttpPost]
        public async Task<ActionResult<long>> CreateProject([FromBody] ProjectSettings projectSettings)
        {
            long id = await ProjectService.CreateProject(projectSettings);
            return CreatedAtAction(nameof(Get), id, new {projectId = id});
        }

        [HttpPut("{projectId}/from-template-csv")]
        public async Task<ActionResult> UpdateFromTemplate([FromRoute] long projectId)
        {
            using StreamReader sr = new StreamReader(Request.Body, Encoding.UTF8);
            string templateCsv = await sr.ReadToEndAsync();
            await TemplateService.ImportTemplateIntoProject(templateCsv, projectId);
            return Ok();
        }

        [HttpPost("from-template")]
        public async Task<ActionResult> CreateProjectFromTemplate([FromQuery] long templateId, [FromBody] ProjectSettings projectSettings)
        {
            string templateCsv = await TemplateService.GetProjectAsTemplateCSV(templateId);
            long projectId = await ProjectService.CreateProject(projectSettings);
            await TemplateService.ImportTemplateIntoProject(templateCsv, projectId);
                
            return Ok();
        }
    }
}