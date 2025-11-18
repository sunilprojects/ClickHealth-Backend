using ClickHealthBackend.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.Services.Interfaces
{
    public interface IContentMetricsService
    {
        Task<List<CityPerformanceDTO>> GetCityPerformanceWithFiltersAsync(PerformanceFilterDTO filter);
        Task<ContentEffectivenessDTO> GetContentEffectivenessAsync(PerformanceFilterDTO filter);
    }
}
