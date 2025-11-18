using ClickHealthBackend.DTOs;

namespace ClickHealthBackend.Services.Interfaces
{
    public interface ICampaignDashboardService
    {
        Task<DashboardDTO> GetMarketingDashboardAsync();
    }
}
