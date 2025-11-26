using ClickHealthBackend.DTOs;
using ClickHealthBackend.Repositories.Implementations;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClickHealthBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignDashboardController : ControllerBase
    {
        private readonly ICampaignDashboardService _service;
        private readonly IContentRepository _contentRepository;

        private readonly IContentService contentService;

        public CampaignDashboardController(ICampaignDashboardService service, IContentRepository contentRepository)
        {
            _service = service;
            _contentRepository = contentRepository;   // ✔️ Correct assignment
        }


        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardDTO>> GetDashboard()
        {
            var dashboard = await _service.GetMarketingDashboardAsync();
            return Ok(dashboard);
        }

        [HttpGet("approved")]
        public async Task<IActionResult> GetApprovedContent()
        {
            var contents = await _contentRepository.GetApprovedContentAsync();
            return Ok(contents);
        }

    }
}
