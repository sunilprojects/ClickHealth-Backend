using ClickHealthBackend.DTOs;
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

        public CampaignDashboardController(ICampaignDashboardService service)
        {
            _service = service;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardDTO>> GetDashboard()
        {
            var dashboard = await _service.GetMarketingDashboardAsync();
            return Ok(dashboard);
        }
    }
}
