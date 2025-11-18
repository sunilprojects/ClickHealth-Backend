using ClickHealthBackend.Data;
using ClickHealthBackend.DTOs;
using ClickHealthBackend.Models;
using ClickHealthBackend.Services.Interfaces;
using MongoDB.Driver;

namespace ClickHealthBackend.Services.Implementations
{
    public class CampaignMetricsService : ICampaignMetricsService
    {
        private readonly IMongoCollection<CampaignMetrics> _metrics;
        private readonly IMongoCollection<User> _users;

        public CampaignMetricsService(MongoDbContext context)
        {
            _metrics = context.CampaignMetrics;
            _users = context.Users;
        }

        public async Task<List<TerritoryPerformanceDTO>> GetRegionalPerformanceHeatMapAsync(string campaignId)
        {
            // 1. Get all metrics for the campaign
            var campaignMetrics = await _metrics.Find(m => m.CampaignId == campaignId).ToListAsync();

            // 2. Group by territory and calculate aggregate performance
            var territoryMetrics = campaignMetrics
                .GroupBy(m => m.Territory)
                .Where(g => g.Key != null)
                .Select(g => new
                {
                    Territory = g.Key,
                    TotalActiveHCP = g.Sum(m => m.HcpActiveCount),
                    TotalInvites = g.Sum(m => m.PatientInviteCount),
                    TotalCompletions = g.Sum(m => m.PatientCompletionCount),
                })
                .ToList();

            var results = new List<TerritoryPerformanceDTO>();
            foreach (var tm in territoryMetrics)
            {
                // Simple calculation for example purposes
                double patientConversionRate = tm.TotalInvites > 0 ? (double)tm.TotalCompletions / tm.TotalInvites : 0;
                string performance = patientConversionRate > 0.5 ? "High" : (patientConversionRate > 0.2 ? "Medium" : "Low");

                // Get total HCPs in that territory (a separate query in a real app)
                var hcpCount = await _users.CountDocumentsAsync(u => u.Territory == tm.Territory && u.Role.ToString() == "HCP");

                results.Add(new TerritoryPerformanceDTO
                {
                    Territory = tm.Territory,
                    EngagementPercentage = patientConversionRate * 100, // Using conversion rate as a proxy
                    ActiveCampaignsCount = 1, // Placeholder
                    HCPAmount = (int)hcpCount,
                    PatientsReached = tm.TotalInvites,
                    OverallPerformance = performance
                });
            }

            return results;
        }
    }
}
