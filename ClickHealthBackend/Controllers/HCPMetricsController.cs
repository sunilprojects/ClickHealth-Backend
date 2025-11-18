using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClickHealthBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HCPMetricsController : ControllerBase
    {
        private readonly IHCPRepository _hcpRepo;

        public HCPMetricsController(IHCPRepository hcpRepo)
        {
            _hcpRepo = hcpRepo;
        }

        // 2. Total HCP Onboarded (get API)
        [HttpGet("total-onboarded")]
        public async Task<ActionResult<long>> GetTotalHCPOnboarded()
        {
            var count = await _hcpRepo.GetTotalHCPOnboardedAsync();
            return Ok(count);
        }

        // 4. Monthly Active Users (HCPs) (get)
        [HttpGet("monthly-active")]
        public async Task<ActionResult<long>> GetMonthlyActiveHCPs()
        {
            var count = await _hcpRepo.GetMonthlyActiveHCPCountAsync();
            return Ok(count);
        }

        // 6. Most Engaged HCPs (get)
        [HttpGet("most-engaged")]
        public async Task<ActionResult<List<User>>> GetMostEngagedHCPs()
        {
            // Assuming default limit of 10 and descending order for "Most Engaged"
            var hcps = await _hcpRepo.GetEngagedHCPsAsync(limit: 10, ascending: false);
            return Ok(hcps);
        }

        // 7. Needs Attention (Lowest Engaged) (get)
        [HttpGet("needs-attention")]
        public async Task<ActionResult<List<User>>> GetLowestEngagedHCPs()
        {
            // Set ascending=true for "Lowest Engaged"
            var hcps = await _hcpRepo.GetEngagedHCPsAsync(limit: 10, ascending: true);
            return Ok(hcps);
        }

        // 5. Avg. Engagement Score (get) - Requires complex aggregation.
        // Simplified implementation: Returns a static average for demonstration.
        [HttpGet("average-score")]
        public ActionResult<double> GetAverageEngagementScore()
        {
            // NOTE: A real implementation requires complex MongoDB Aggregation 
            // over ContentEngagement and HCPActivity collections.
            return Ok(4.75); // Dummy value
        }
    }
}
