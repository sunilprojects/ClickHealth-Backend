using ClickHealthBackend.DTOs;
using ClickHealthBackend.Models;
using ClickHealthBackend.Services.Interfaces;
using MongoDB.Driver;

namespace ClickHealthBackend.Services.Implementations
{
    public class PatientService : IPatientInviteService
    {
        private readonly IMongoCollection<Patient> _patients;
        private readonly IMongoCollection<PatientInvite> _invites;
        private readonly IMongoCollection<Campaign> _campaigns;
        private readonly IMongoCollection<Content> _contents;

        public PatientService(IMongoDatabase db)
        {
            _patients = db.GetCollection<Patient>("Patients");
            _invites = db.GetCollection<PatientInvite>("PatientInvites");
            _campaigns = db.GetCollection<Campaign>("Campaigns");
            _contents = db.GetCollection<Content>("Contents");
        }

        // ---------------------------------------------------------
        // CREATE OR GET PATIENT
        // ---------------------------------------------------------
        public async Task<Patient> CreateOrGetPatientAsync(Patient patient)
        {
            var existing = await _patients
                .Find(x => x.Email == patient.Email)
                .FirstOrDefaultAsync();

            if (existing != null)
                return existing;

            await _patients.InsertOneAsync(patient);
            return patient;
        }

        // ---------------------------------------------------------
        // CREATE INVITE
        // ---------------------------------------------------------
        public async Task<PatientInvite> CreateInviteAsync(
            string patientId,
            string patientCustomId,
            string campaignId,
            string hcpId)
        {
            var invite = new PatientInvite
            {
                PatientId = patientId,
                PatientCustomId = patientCustomId,
                CampaignId = campaignId,
                HcpId = hcpId,
                InvitedAt = DateTime.UtcNow,
                ViewedContentIds = new List<string>() // Ensure not null
            };

            await _invites.InsertOneAsync(invite);
            return invite;
        }

        // ---------------------------------------------------------
        // GET PATIENT BY CUSTOM ID
        // ---------------------------------------------------------
        public async Task<Patient> GetPatientByCustomIdAsync(string patientCustomId)
        {
            return await _patients
                .Find(x => x.PatientCustomId == patientCustomId)
                .FirstOrDefaultAsync();
        }

        // ---------------------------------------------------------
        // GET INVITE
        // ---------------------------------------------------------
        public async Task<PatientInvite> GetInviteAsync(string patientCustomId, string campaignId)
        {
            return await _invites.Find(x =>
                x.PatientCustomId == patientCustomId &&
                x.CampaignId == campaignId
            ).FirstOrDefaultAsync();
        }

        // ---------------------------------------------------------
        // GET CONTENTS FOR CAMPAIGN
        // ---------------------------------------------------------
        public async Task<List<Content>> GetCampaignContentsAsync(string campaignId)
        {
            var campaign = await _campaigns
                .Find(x => x.CampaignCustomId == campaignId)
                .FirstOrDefaultAsync();

            if (campaign == null)
                return new List<Content>();

            return await _contents
                .Find(c => campaign.ContentIds.Contains(c.ContentCustomId))
                .ToListAsync();
        }

        // ---------------------------------------------------------
        // RECORD CONTENT VIEW
        // ---------------------------------------------------------
        public async Task RecordContentViewAsync(string patientId, string campaignId, string contentId)
        {
            var update = Builders<PatientInvite>.Update
                .AddToSet("ViewedContentIds", contentId);

            await _invites.UpdateOneAsync(
                x => x.PatientId == patientId && x.CampaignId == campaignId,
                update
            );
        }

        // ---------------------------------------------------------
        // GET PATIENT DASHBOARD
        // ---------------------------------------------------------
        public async Task<PatientDashboardDTO> GetDashboardAsync(string patientCustomId, string campaignId)
        {
            var patient = await GetPatientByCustomIdAsync(patientCustomId);
            var invite = await GetInviteAsync(patientCustomId, campaignId);
            var contents = await GetCampaignContentsAsync(campaignId);

            return new PatientDashboardDTO
            {
                PatientName = patient.Name,
                PatientCustomId = patient.PatientCustomId,
                CampaignId = campaignId,
                HcpId = invite.HcpId,

                Contents = contents.Select(x => new ContentDTO
                {
                    MedicalName = x.Therapy,
                    ContentLanguage = x.Language,
                    ContentDescription = x.Description,
                    PdfUrl = x.FileUrl,
                    VideoUrl = x.ThumbnailUrl,
                    ReviewOn = x.ReviewDate,
                    ExpiresOn = x.ExpiryDate,
                    Status = x.Status
                }).ToList(),

                ViewedContentIds = invite.ViewedContentIds ?? new List<string>()
            };
        }

        public Task SendCampaignToPatientsAsync(string campaignId, string hcpId, string specialty)
        {
            throw new NotImplementedException();
        }
    }
}
