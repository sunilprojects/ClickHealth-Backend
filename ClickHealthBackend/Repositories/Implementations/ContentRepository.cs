using ClickHealthBackend.Data;
using ClickHealthBackend.DTOs;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Enums;
using MongoDB.Driver;

namespace ClickHealthBackend.Repositories.Implementations
{
    public class ContentRepository : IContentRepository
    {
        private readonly IMongoCollection<Content> _content;
        private readonly IMongoCollection<PatientEngagement> _patientEngagements;
        private readonly IMongoCollection<User> _users;

        public ContentRepository(MongoDbContext context)
        {
            _content = context.Contents;
            _patientEngagements = context.PatientEngagement;
            _users = context.Users;
        }

        // --- Simple Fetch ---
        public async Task<List<Content>> GetAllContentAsync() =>
            await _content.Find(_ => true).ToListAsync();

        // --- Metrics Aggregation ---
        public async Task<List<ContentMetricsDTO>> GetContentMetricsAsync(PerformanceFilterDTO filter)
        {
            var engagementFilter = Builders<PatientEngagement>.Filter.Empty;

            if (filter.StartDate.HasValue)
                engagementFilter &= Builders<PatientEngagement>.Filter.Gte(a => a.ViewedAt, filter.StartDate.Value);
            if (filter.EndDate.HasValue)
                engagementFilter &= Builders<PatientEngagement>.Filter.Lte(a => a.ViewedAt, filter.EndDate.Value);

            var engagements = await _patientEngagements.Find(engagementFilter).ToListAsync();

            var grouped = engagements
                .GroupBy(a => a.ContentId)
                .Select(g => new
                {
                    ContentId = g.Key,
                    Completions = g.Count(a => a.CompletedAt.HasValue),
                    TotalDwellTimeSeconds = g.Sum(a => a.DurationSeconds),
                    TotalShares = g.Count(a => !string.IsNullOrEmpty(a.InviteCode)),
                    TotalViews = g.Count()
                })
                .ToList();

            var contentIds = grouped.Select(g => g.ContentId).ToList();
            var contentMetadata = await _content.Find(c => contentIds.Contains(c.ContentId)).ToListAsync();

            var contentMap = contentMetadata.ToDictionary(c => c.ContentId, c => c);

            var metrics = grouped
                .Select(g =>
                {
                    if (!contentMap.TryGetValue(g.ContentId, out var content))
                        return null;

                    if (!string.IsNullOrEmpty(filter.Language) && content.Language != filter.Language)
                        return null;

                    double avgDwellTimeMins = g.TotalViews > 0 ? (double)g.TotalDwellTimeSeconds / 60 / g.TotalViews : 0;
                    double shareRate = g.TotalViews > 0 ? (double)g.TotalShares / g.TotalViews * 100 : 0;

                    return new ContentMetricsDTO
                    {
                        ContentId = content.ContentId,
                        Title = content.Therapy,
                        ContentType = content.ContentType.ToString(),
                        Language = content.Language,
                        Completions = g.Completions,
                        AverageDwellTimeMinutes = Math.Round(avgDwellTimeMins, 2),
                        ShareRatePercentage = Math.Round(shareRate, 2),
                    };
                })
                .Where(m => m != null)
                .ToList();

            Console.WriteLine("StartDate: " + filter.StartDate);
            Console.WriteLine("EndDate: " + filter.EndDate);
            Console.WriteLine("Language: " + filter.Language);

            Console.WriteLine("Engagements Count: " + engagements.Count);
            Console.WriteLine("Grouped Count: " + grouped.Count);
            //Console.WriteLine("City Result Count: " + cityResults.Count);


            return metrics!;
        }

        // --- City Performance ---
        public async Task<List<CityPerformanceDTO>> GetCityPerformanceAsync(PerformanceFilterDTO filter)
        {
            var allEngagements = await _patientEngagements.Find(_ => true).ToListAsync();

            var cityGroup = allEngagements
                .Where(a => !string.IsNullOrEmpty(a.City))
                .GroupBy(a => a.City)
                .Select(g => new
                {
                    City = g.Key,
                    TotalCompletions = g.Count(a => a.CompletedAt.HasValue),
                    TotalEngagements = g.Count(),
                    LanguageMix = g.Any(a => !string.IsNullOrEmpty(a.Language)) ? "Mixed" : "English"
                })
                .Where(g => string.IsNullOrEmpty(filter.City) || g.City == filter.City)
                .ToList();

            return cityGroup.Select(g => new CityPerformanceDTO
            {
                City = g.City,
                TotalCompletions = g.TotalCompletions,
                EngagementPercentage = Math.Round((double)g.TotalCompletions / g.TotalEngagements * 100, 2),
                LanguageMix = g.LanguageMix
            }).ToList();
        }

        // --- CRUD ---
        public async Task<Content> GetByIdAsync(string id) =>
            await _content.Find(c => c.ContentId == id).FirstOrDefaultAsync();

        public async Task<IEnumerable<Content>> GetPendingContentAsync() =>
            await _content.Find(c => c.Status == ContentStatus.Pending).ToListAsync();

        public async Task<IEnumerable<Content>> GetAllAsync() =>
            await _content.Find(_ => true).ToListAsync();

        public async Task CreateAsync(Content content) =>
            await _content.InsertOneAsync(content);

        public async Task<bool> UpdateAsync(Content content)
        {
            var result = await _content.ReplaceOneAsync(c => c.ContentId == content.ContentId, content);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
