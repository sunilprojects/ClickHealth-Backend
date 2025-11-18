using ClickHealthBackend.Enums;
using MongoDB.Bson.Serialization; // Required for BsonSerializer.Deserialize
using System.Text.Json;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Interfaces;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClickHealthBackend.Services.Implementation
{
    public class HCPService : IHCPService
    {
        private readonly IContentRepository _contentRepository;
        private readonly IPatientInviteRepository _patientInviteRepository;
        private readonly IHCPActivityRepository _hcpActivityRepository;
        private readonly IMRActivityRepository _mrActivityRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IEmailService _emailService;
        // NOTE: ICampaignAssetRepository logic is integrated conceptually via the helper method

        public HCPService(
            IContentRepository contentRepository,
            IPatientInviteRepository patientInviteRepository,
            IHCPActivityRepository hcpActivityRepository,
            IMRActivityRepository mrActivityRepository,
            ICampaignRepository campaignRepository,
            IEmailService emailService)
        {
            _contentRepository = contentRepository;
            _patientInviteRepository = patientInviteRepository;
            _hcpActivityRepository = hcpActivityRepository;
            _mrActivityRepository = mrActivityRepository;
            _campaignRepository = campaignRepository;
            _emailService = emailService;
        }

        // ========================================================
        // 1. GET ASSIGNED CONTENT (ALIGNED WITH MR SHARE)
        // ========================================================
        public async Task<IEnumerable<Content>> GetAssignedCampaignContentAsync(string hcpUserId)
        {
            // 1. Find all MRActivity records that represent a campaign assignment
            var assignedActivities = (await _mrActivityRepository.GetAllAsync())
                .Where(a => a.HcpUserId == hcpUserId && a.MrActivityType == MRActivityType.PackShare)
                .ToList();

            if (!assignedActivities.Any())
            {
                return Enumerable.Empty<Content>();
            }

            // 2. Get the unique list of assigned Campaign IDs
            var assignedCampaignIds = assignedActivities.Select(a => a.CampaignId).Distinct().ToList();

            // 3. Find all Content IDs associated with these Campaigns (Conceptual step)
            var contentIds = await GetContentIdsByCampaignsAsync(assignedCampaignIds);

            // 4. Retrieve and filter the Content objects
            var allContent = await _contentRepository.GetAllAsync();

            var assignedContent = allContent
                // Filter by ContentIds obtained from CampaignAsset lookup AND ensure content is approved
                .Where(c => contentIds.Contains(c.ContentId) && c.Status == ContentStatus.Approved)
                .ToList();

            // The Content objects returned here include the 'fileUrl' (S3 link).
            return assignedContent;
        }

        // ========================================================
        // 2. PATIENT INVITATION (SHARES CAMPAIGN LINK)
        // ========================================================
        public async Task<string> GeneratePatientInviteLinkAsync(string hcpUserId, string campaignId, string patientEmail)
        {
            // 1. Verification: Ensure campaign is approved/active
            var campaign = await _campaignRepository.GetByIdAsync(campaignId);
            if (campaign == null || campaign.Status != CampaignStatus.Active)
            {
                return null;
            }

            // 2. Create Invite Record (LINKED TO CAMPAIGN)
            var inviteCode = Guid.NewGuid().ToString("N");
            var invite = new Patient
            {
                InviteCode = inviteCode,
                HcpUserId = hcpUserId,
                CampaignId = campaignId,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsActive = true,
                MaxUses = 1
            };
            await _patientInviteRepository.CreateAsync(invite);

            // 3. Send Email (Communication)
            string subject = "An Educational Resource from Your Healthcare Provider";
            string body = $"Dear {campaign.Name},\n\nYour doctor has shared an educational resource (Campaign: {campaign.Name}) with you. Please click the link below to view the content:\n\nhttps://your-app-url.com/patient-portal?inviteCode={inviteCode}\n\nThis link is private and will expire in 7 days.\n\nThank you,\nClickHealth Team";
            await _emailService.SendEmailAsync(patientEmail, subject, body);

            // 4. Log Activity (HCP's sharing action)
            await _hcpActivityRepository.CreateAsync(new HCPActivity
            {
                HcpUserId = hcpUserId,
                HcpActivityType = HCPActivityType.Share,
                CampaignId = campaignId, // Log the Campaign ID
                Timestamp = DateTime.UtcNow,
            });

            return inviteCode;
        }

        // ========================================================
        // 3. QUIZ LOGGING
        // ========================================================
        public async Task<bool> LogQuizCompletionAsync(string hcpUserId, string contentId, Dictionary<string, object> quizResponses)
        {
            // --- CRITICAL FIX: Safe Serialization to BSON ---

            // 1. Convert the problematic Dictionary<string, object> into a pure JSON string.
            var jsonString = JsonSerializer.Serialize(quizResponses);

            // 2. Use the BsonSerializer to safely parse the JSON string into a BsonDocument.
            // This correctly resolves JSON primitives (like "1.0", 4, 1) into native BSON types.
            BsonDocument bsonMetadata;
            try
            {
                bsonMetadata = BsonSerializer.Deserialize<BsonDocument>(jsonString);
            }
            catch (Exception ex)
            {
                // Log error if serialization fails (e.g., malformed JSON structure)
                Console.WriteLine($"BSON Serialization Error: {ex.Message}");
                return false;
            }

            // --- End of Fix ---

            // Log the quiz completion and responses for the HCP
            await _hcpActivityRepository.CreateAsync(new HCPActivity
            {
                HcpUserId = hcpUserId,
                ContentId = contentId,
                HcpActivityType = HCPActivityType.Quiz,
                Timestamp = DateTime.UtcNow,
                Metadata = bsonMetadata // Use the safely converted BsonDocument
            });
            return true;
        }

        // ========================================================
        // Helper to integrate CampaignAsset lookup (Conceptual)
        // ========================================================
        private Task<List<string>> GetContentIdsByCampaignsAsync(List<string> campaignIds)
        {
            // Placeholder: This method MUST be replaced by ICampaignAssetRepository logic.
            // Returning an empty list here allows the service to compile and run for testing.
            return Task.FromResult(new List<string>());
        }

        public Task<IEnumerable<Content>> GetApprovedContentAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<HCPContentDto>> GetAssignedContentDtoAsync(string hcpUserId)
        {
            var content = await GetAssignedCampaignContentAsync(hcpUserId);

            return content.Select(c => new HCPContentDto
            {
                ContentId = c.ContentId,
                ContentName = c.Therapy,              // ✔ Correct: Therapy = medical_name
                Language = c.Language,
                PdfUrl = c.FileUrl,                   // ✔ upload_pdf
                VideoUrl = c.ThumbnailUrl,            // ✔ video_url
                CampaignId = null,                    // ❗ Your Content model has NO CampaignId
                AssignedPatients = 0
            });
        }

    }
}