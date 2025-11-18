using ClickHealthBackend.DTOs;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Interfaces;

namespace ClickHealthBackend.Services.Implementations
{
    public class CampaignDashboardService : ICampaignDashboardService
    {
        private readonly ICampaignDashboardRepository _repo;

        public CampaignDashboardService(ICampaignDashboardRepository repo)
        {
            _repo = repo;
        }

        public async Task<DashboardDTO> GetMarketingDashboardAsync()
        {
            var activeCount = await _repo.GetActiveCampaignsCountAsync();
            var breakdown = await _repo.GetCampaignBreakdownAsync();
            var totalReach = await _repo.GetTotalReachAsync();
            var doctorShares = await _repo.GetDoctorSharesAsync();
            var completionRate = await _repo.GetCompletionRateAsync();

            return new DashboardDTO
            {
                ActiveCampaigns = activeCount,
                CampaignBreakdown = string.Join(", ", breakdown.Select(kvp => $"{kvp.Value} {kvp.Key}")),
                TotalReach = totalReach,
                ReachDetail = "Patients invited this month",
                DoctorShares = doctorShares,
                DoctorSharesDetail = "HCPs actively sharing content",
                CompletionRate = completionRate,
                CompletionRateDetail = "Average across all campaigns",
                //ROISignals = new List<ROIData>
                //{
                //    new ROIData { Label = "ROI Improvement", Value = "+15%", Detail="vs last quarter", IsPositive=true },
                //    new ROIData { Label = "Patient Engagement", Value = "+22%", Detail="week over week", IsPositive=true },
                //    new ROIData { Label = "HCP Satisfaction", Value="4.2/5", Detail="avg rating", IsPositive=true }
                //},
                //Insights = new List<InsightData>
                //{
                //    new InsightData { Icon="💡", Text="Compare EN vs Hindi completion rates in Mumbai" },
                //    new InsightData { Icon="📉", Text="Where did we underperform vs last week?" },
                //    new InsightData { Icon="🤖", Text="Recommend next action — AI suggestion" }
                //}
            };
        }
    }
}
