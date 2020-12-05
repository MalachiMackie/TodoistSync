using System.Collections.Generic;
using System.Linq;
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

        private ICommentsService CommentsService { get; }

        private IItemService ItemService { get; }

        private ISectionService SectionService { get; }

        public ProjectsController(IProjectService projectService,
            ICommentsService commentsService,
            IItemService itemService,
            ISectionService sectionService)
        {
            ProjectService = projectService;
            CommentsService = commentsService;
            ItemService = itemService;
            SectionService = sectionService;
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

        [HttpPost]
        public async Task<ActionResult<long>> CreateProject([FromBody] ProjectSettings projectSettings)
        {
            var projectId = await ProjectService.CreateProject(projectSettings);
            if (projectSettings.FromTemplateId.HasValue)
            {
                var templateId = projectSettings.FromTemplateId.Value;
                await CommentsService.CreateComment(new Comment{Content = $"TemplateId:{templateId}", ProjectId = projectId});
                var templateItems = await ItemService.GetItemsInProject(templateId);
                var sections = await SectionService.GetSectionsAsync(templateId);
                var firstSection = sections.First();
                var firstSectionItems = templateItems.Where(x => x.SectionId == firstSection.Id)
                    .ToList();

                firstSectionItems.ForEach(x =>
                    {
                        x.SectionId = null;
                        x.ProjectId = projectId;
                        x.DueDateTime = x.Due?.Datetime;
                        x.DueDate = x.Due?.Datetime == null ? x.Due?.Date : null;
                    });
                await ItemService.PostItems(firstSectionItems);
            }

            return CreatedAtAction(nameof(Get), projectId, new {projectId});
        }
    }
}
