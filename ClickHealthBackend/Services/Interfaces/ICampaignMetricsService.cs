using ClickHealthBackend.DTOs;

namespace ClickHealthBackend.Services.Interfaces
{
    public interface ICampaignMetricsService
    {
        Task<List<TerritoryPerformanceDTO>> GetRegionalPerformanceHeatMapAsync(string campaignId);
    }
}
