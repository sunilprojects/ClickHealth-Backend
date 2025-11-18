using ClickHealthBackend.Data;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using MongoDB.Driver;

namespace ClickHealthBackend.Repositories.Implementations
{
    public class CampaignDashboardRepository : ICampaignDashboardRepository
    {
        private readonly IMongoCollection<Campaign> _campaigns;

        public CampaignDashboardRepository(MongoDbContext context)
        {
            _campaigns = context.Campaigns;
        }

        public async Task<int> GetActiveCampaignsCountAsync()
        {
            var now = DateTime.UtcNow;
            return (int)await _campaigns.CountDocumentsAsync(c => c.StartDate <= now && c.EndDate >= now);
        }

        public async Task<Dictionary<string, int>> GetCampaignBreakdownAsync()
        {
            var campaigns = await _campaigns.Find(_ => true).ToListAsync();
            return campaigns
                .GroupBy(c => c.Therapy)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<int> GetTotalReachAsync()
        {
            var campaigns = await _campaigns.Find(_ => true).ToListAsync();
            // Assuming TargetMetrics contains key "Reach"
            return campaigns.Sum(c => c.TargetMetrics != null && c.TargetMetrics.Contains("Reach")
                                      ? c.TargetMetrics["Reach"].AsInt32
                                      : 0);
        }

        public async Task<int> GetDoctorSharesAsync()
        {
            var campaigns = await _campaigns.Find(_ => true).ToListAsync();
            // Assuming TargetMetrics contains key "DoctorShares"
            return campaigns.Sum(c => c.TargetMetrics != null && c.TargetMetrics.Contains("DoctorShares")
                                      ? c.TargetMetrics["DoctorShares"].AsInt32
                                      : 0);
        }

        public async Task<int> GetCompletionRateAsync()
        {
            var campaigns = await _campaigns.Find(_ => true).ToListAsync();
            var total = campaigns.Sum(c => c.TargetMetrics != null && c.TargetMetrics.Contains("Target")
                                           ? c.TargetMetrics["Target"].AsInt32 : 0);
            var completed = campaigns.Sum(c => c.TargetMetrics != null && c.TargetMetrics.Contains("Completed")
                                              ? c.TargetMetrics["Completed"].AsInt32 : 0);

            return total > 0 ? (int)((completed * 100.0) / total) : 0;
        }
    }
}
