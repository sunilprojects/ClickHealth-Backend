using ClickHealthBackend.DTOs;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketingContentController : ControllerBase
    {
        private readonly IContentMetricsService _metricsService;

        public MarketingContentController(IContentMetricsService metricsService)
        {
            _metricsService = metricsService;
        }

        // --- 1. City Performance API (GET with Filters) ---
        // Example: GET api/marketingcontent/city-performance?city=Mumbai&language=English
        [HttpGet("city-performance")]
        public async Task<ActionResult<List<CityPerformanceDTO>>> GetCityPerformance([FromQuery] PerformanceFilterDTO filter)
        {
            var result = await _metricsService.GetCityPerformanceWithFiltersAsync(filter);

            if (result == null || result.Count == 0)
            {
                return NotFound("No city performance data found based on the provided filters.");
            }
            return Ok(result);
        }


        // --- 2. Content Effectiveness API (GET with Filters) ---
        // Example: GET api/marketingcontent/effectiveness?language=Hindi
        [HttpGet("effectiveness")]
        public async Task<ActionResult<ContentEffectivenessDTO>> GetContentEffectiveness([FromQuery] PerformanceFilterDTO filter)
        {
            var result = await _metricsService.GetContentEffectivenessAsync(filter);

            if (result == null)
            {
                return NotFound("No content effectiveness data found based on the provided filters.");
            }
            return Ok(result);
        }
    }
}
