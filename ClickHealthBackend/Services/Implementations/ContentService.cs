using ClickHealthBackend.Data;
using ClickHealthBackend.DTOs;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Interfaces;
using MongoDB.Driver;

namespace ClickHealthBackend.Services.Implementations
{
    public class ContentService : IContentService
    {
        private readonly IContentRepository _contentRepository;
        private readonly IMongoCollection<Content> _contentCollection;

        public ContentService(MongoDbContext context, IContentRepository contentRepository)
        {
            _contentCollection = context.Contents;
            _contentRepository = contentRepository;
        }

        // Generate next ContentId like CT001
        public async Task<string> GenerateContentId()
        {
            var last = await _contentCollection
                .Find(_ => true)
                .SortByDescending(c => c.ContentCustomId)
                .FirstOrDefaultAsync();

            int next = 1;

            if (last != null)
            {
                next = int.Parse(last.ContentCustomId.Replace("CT", "")) + 1;
            }

            return $"CT{next:D3}";
        }

        public async Task<string> GeneratePdfId()
        {
            var last = await _contentCollection
                .Find(c => c.PdfCustomId != null)
                .SortByDescending(c => c.PdfCustomId)
                .FirstOrDefaultAsync();

            int next = 1;

            if (last != null)
                next = int.Parse(last.PdfCustomId.Replace("PDF", "")) + 1;

            return $"PDF{next:D3}";
        }

        public async Task<string> GenerateVideoId()
        {
            var last = await _contentCollection
                .Find(c => c.VideoCustomId != null)
                .SortByDescending(c => c.VideoCustomId)
                .FirstOrDefaultAsync();

            int next = 1;

            if (last != null)
                next = int.Parse(last.VideoCustomId.Replace("VID", "")) + 1;

            return $"VID{next:D3}";
        }

        // Upload content
        public async Task<string> UploadContentAsync(
            ContentDTO dto,
            string uploaderId,
            string uploaderName)
        {
            var content = new Content
            {
                ContentId = Guid.NewGuid().ToString(),
                ContentCustomId = await GenerateContentId(),

                PdfCustomId = await GeneratePdfId(),
                VideoCustomId = await GenerateVideoId(),

                Therapy = dto.MedicalName,
                Language = dto.ContentLanguage,
                Description = dto.ContentDescription,

                FileUrl = dto.PdfUrl,
                ThumbnailUrl = dto.VideoUrl,

                ReviewDate = dto.ReviewOn,
                ExpiryDate = dto.ExpiresOn,

                UploadedAt = DateTime.UtcNow,
                Status = ContentStatus.Pending,

                UploadedByUserId = uploaderId,
                UploadedByUserName = uploaderName
            };

            await _contentRepository.CreateAsync(content);

            return content.ContentId;
        }

        // Approve or Reject content
        public async Task<bool> UpdateContentStatusByCustomIdAsync(
            string contentCustomId,
            ContentStatus newStatus,
            string approverCustomId,
            string approverName,
            string? notes)
        {
            return await _contentRepository.UpdateStatusByCustomIdAsync(
                contentCustomId,
                newStatus,
                approverCustomId,
                approverName,
                notes
            );
        }

        // Workflow view for uploader
        public async Task<List<ContentWorkflowDTO>> GetContentWorkflowByUploaderAsync(string uploaderId)
        {
            var contents = await _contentCollection
                .Find(c => c.UploadedByUserId == uploaderId)
                .ToListAsync();

            return contents.Select(c => new ContentWorkflowDTO
            {
                ContentId = c.ContentId,
                Content = c.Description,
                ContentCreator = c.UploadedByUserName,
                UploadedAt = c.UploadedAt,
                Status = c.Status.ToString(),
                Approver = c.Metadata?.ApprovedByUserName,
                Notes = c.Metadata?.ApproverNotes
            }).ToList();
        }

        public async Task<bool> UpdateContentStatusAsync(string contentId, ContentStatus newStatus, string approverName, string? notes)
        {
            var filter = Builders<Content>.Filter.Eq(c => c.ContentId, contentId);

            var update = Builders<Content>.Update
                .Set(c => c.Status, newStatus)
                .Set(c => c.Metadata.ApprovedByUserName, approverName)
                .Set(c => c.Metadata.ApproverNotes, notes)
                .Set(c => c.ApprovedAt, DateTime.UtcNow);

            var result = await _contentCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        public Task<List<MedicalContentResponseDto>> GetContentsByStatusAsync(ContentStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
