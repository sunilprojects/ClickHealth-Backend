using ClickHealthBackend.Data;
using ClickHealthBackend.DTOs;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using ClickHealthBackend.Services.Interfaces;
using MongoDB.Driver;

namespace ClickHealthBackend.Services.Implementations
{
    public class ContentService : IContentService
    {
        private readonly IMongoCollection<Content> _contentCollection;

        public ContentService(MongoDbContext context)
        {
            _contentCollection = context.Contents;
        }


        public async Task<string> GenerateContentId()
        {
            var sort = Builders<Content>.Sort.Descending(x => x.ContentCustomId);

            var lastContent = await _contentCollection
                .Find(_ => true)
                .Sort(sort)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastContent != null && !string.IsNullOrEmpty(lastContent.ContentCustomId))
            {
                string numericPart = lastContent.ContentCustomId.Replace("CT", "");
                nextNumber = int.Parse(numericPart) + 1;
            }

            return $"CT{nextNumber:D3}";
        }





        // ✅ Upload content
        public async Task<string> UploadContentAsync(ContentDTO newContentDto, string uploadedByUserId, string uploadedByUserName)
        {
            if (newContentDto == null)
                throw new ArgumentNullException(nameof(newContentDto));

            var content = new Content
            {
                ContentId = Guid.NewGuid().ToString(),

                ContentCustomId = await GenerateContentId(),

                Therapy = newContentDto.MedicalName,
                Language = newContentDto.ContentLanguage,
                Description = newContentDto.ContentDescription,
                FileUrl = newContentDto.PdfUrl,
                ThumbnailUrl = newContentDto.VideoUrl,
                ReviewDate = newContentDto.ReviewOn,
                ExpiryDate = newContentDto.ExpiresOn,
                UploadedAt = DateTime.UtcNow,
                Status = ContentStatus.Pending, // ✅ Always start as pending
                UploadedByUserId = uploadedByUserId,
                UploadedByUserName = uploadedByUserName
            };

            await _contentCollection.InsertOneAsync(content);
            return content.ContentId;
        }

        // ✅ Get all content by status
        public async Task<List<MedicalContentResponseDto>> GetContentsByStatusAsync(ContentStatus status)
        {
            var filter = Builders<Content>.Filter.Eq(c => c.Status, status);

            var contents = await _contentCollection.Find(filter).ToListAsync();

            return contents.Select(c => new MedicalContentResponseDto
            {
                ContentId = c.ContentId,
                MedicalName = c.Therapy,
                ContentLanguage = c.Language,
                Description = c.Description,
                ViewPdf = c.FileUrl,
                ViewVideo = c.ThumbnailUrl,
                StartDate = c.ReviewDate,
                EndDate = c.ExpiryDate,
                Status = c.Status
            }).ToList();
        }

        // ✅ Update status (approve/reject)
        public async Task<bool> UpdateContentStatusAsync(string contentId, ContentStatus newStatus, string approverName, string? notes)
        {
            var filter = Builders<Content>.Filter.Eq(c => c.ContentId, contentId);
            var update = Builders<Content>.Update
                .Set(c => c.Status, newStatus)
                .Set(c => c.ApprovedByUserName, approverName)
                .Set(c => c.ApproverNotes, notes)
                .Set(c => c.ApprovedAt, DateTime.UtcNow);

            var result = await _contentCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        // ✅ Workflow by uploader
        public async Task<List<ContentWorkflowDTO>> GetContentWorkflowByUploaderAsync(string uploaderId)
        {
            var filter = Builders<Content>.Filter.Eq(c => c.UploadedByUserId, uploaderId);
            var contents = await _contentCollection.Find(filter).ToListAsync();

            return contents.Select(c => new ContentWorkflowDTO
            {
                ContentId = c.ContentId,
                Content = c.Description,
                ContentCreator = c.UploadedByUserName,
                UploadedAt = c.UploadedAt,
                Status = c.Status.ToString(),
                Approver = c.ApprovedByUserName,
                Notes = c.ApproverNotes
            }).ToList();
        }
    }
}
