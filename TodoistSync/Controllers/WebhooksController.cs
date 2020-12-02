using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoistSync.Models;
using TodoistSync.Services;

namespace TodoistSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhooksController : ControllerBase
    {
        public WebhooksController(IWebhookService webhookService)
        {
            WebhookService = webhookService;
        }

        private IWebhookService WebhookService { get; }
        
        [HttpPost]
        public async Task<ActionResult> Post(WebhookRequest request)
        {
            await WebhookService.ProcessWebhookRequest(request);
            return Ok();
        }
    }
}