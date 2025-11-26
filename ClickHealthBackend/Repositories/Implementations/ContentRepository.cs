using ClickHealthBackend.Data;
using ClickHealthBackend.DTOs;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using MongoDB.Driver;

namespace ClickHealthBackend.Repositories.Implementations
{
    public class ContentRepository : IContentRepository
    {
        private readonly IMongoCollection<Content> _content;
        private readonly IMongoCollection<PatientEngagement> _patientEngagements;

        public ContentRepository(MongoDbContext context)
        {
            _content = context.Contents;
            _patientEngagements = context.PatientEngagement;
        }

        public async Task<Content> GetByCustomIdAsync(string customId)
        {
            return await _content
                .Find(c => c.ContentCustomId == customId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateAsync(Content content)
        {
            await _content.InsertOneAsync(content);
            return true;
        }

        public async Task<bool> UpdateAsync(Content content)
        {
            var result = await _content.ReplaceOneAsync(
                c => c.ContentId == content.ContentId,
                content
            );
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateStatusByCustomIdAsync(
         string contentCustomId,
         ContentStatus newStatus,
         string approverCustomId,
         string approverName,
         string? remarks)
        {
            var filter = Builders<Content>.Filter.Eq(c => c.ContentCustomId, contentCustomId);

            var update = Builders<Content>.Update
                .Set(c => c.Status, newStatus)
                .Set(c => c.Metadata.ApprovedByUserId, approverCustomId)
                .Set(c => c.Metadata.ApprovedByUserName, approverName)
                .Set(c => c.Metadata.ApproverNotes, remarks)
                .Set(c => c.ApprovedAt, DateTime.UtcNow);

            var result = await _content.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }




        public async Task<Content> GetByIdAsync(string id)
        {
            return await _content.Find(c => c.ContentId == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Content>> GetPendingContentAsync()
        {
            return await _content
                .Find(c => c.Status == ContentStatus.Pending)
                .ToListAsync();
        }

        public async Task<IEnumerable<Content>> GetAllAsync()
        {
            return await _content.Find(_ => true).ToListAsync();
        }

        public async Task<List<Content>> GetAllContentAsync()
        {
            return await _content.Find(_ => true).ToListAsync();
        }

        public async Task<List<Content>> GetApprovedContentAsync()
        {
            return await _content
                .Find(c => c.Status == ContentStatus.Approved)
                .ToListAsync();
        }

        Task IContentRepository.CreateAsync(Content content)
        {
            return CreateAsync(content);
        }

        public Task<List<ContentMetricsDTO>> GetContentMetricsAsync(PerformanceFilterDTO filter)
        {
            throw new NotImplementedException();
        }

        public Task<List<CityPerformanceDTO>> GetCityPerformanceAsync(PerformanceFilterDTO filter)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Content>> GetByIdsAsync(List<string> customIds)
        {
            return await _content.Find(x => customIds.Contains(x.ContentCustomId))
                                    .ToListAsync();
        }

    }
}
