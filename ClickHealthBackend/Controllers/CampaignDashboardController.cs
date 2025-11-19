using ClickHealthBackend.DTOs;
<<<<<<< HEAD
=======
using ClickHealthBackend.Repositories.Implementations;
using ClickHealthBackend.Repositories.Interfaces;
>>>>>>> 6d54bde216ffe9ad760fc6fd5b3df6d9b1538c81
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
<<<<<<< HEAD

        public CampaignDashboardController(ICampaignDashboardService service)
        {
            _service = service;
        }

=======
        private readonly IContentRepository _contentRepository;

        private readonly IContentService contentService;

        public CampaignDashboardController(ICampaignDashboardService service, IContentRepository contentRepository)
        {
            _service = service;
            _contentRepository = contentRepository;   // ✔️ Correct assignment
        }


>>>>>>> 6d54bde216ffe9ad760fc6fd5b3df6d9b1538c81
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardDTO>> GetDashboard()
        {
            var dashboard = await _service.GetMarketingDashboardAsync();
            return Ok(dashboard);
        }
<<<<<<< HEAD
=======

        [HttpGet("approved")]
        public async Task<IActionResult> GetApprovedContent()
        {
            var contents = await _contentRepository.GetApprovedContentAsync();
            return Ok(contents);
        }

>>>>>>> 6d54bde216ffe9ad760fc6fd5b3df6d9b1538c81
    }
}
