using System.Threading.Tasks;

namespace TodoistSync.Services
{
    public interface ITemplateService
    {
        Task<string> GetProjectAsTemplateCSV(long projectId);

        Task ImportTemplateIntoProject(string templateCsv, long projectId);
    }
}