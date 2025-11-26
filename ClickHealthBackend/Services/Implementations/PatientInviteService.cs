using ClickHealthBackend.DTOs;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Interfaces;

namespace ClickHealthBackend.Services.Implementations
{
    public class PatientInviteService : IPatientInviteService
    {
        private readonly IPatientInviteRepository _inviteRepo;
        private readonly IPatientRepository _patientRepo;
        private readonly IEmailService _emailService;

        public PatientInviteService(IPatientInviteRepository inviteRepo, IPatientRepository patientRepo, IEmailService emailService)
        {
            _inviteRepo = inviteRepo;
            _patientRepo = patientRepo;
            _emailService = emailService;
        }

        public Task<PatientInvite> CreateInviteAsync(string patientId, string patientCustomId, string campaignId, string hcpId)
        {
            throw new NotImplementedException();
        }

        public Task<Patient> CreateOrGetPatientAsync(Patient patient)
        {
            throw new NotImplementedException();
        }

        public Task<List<Content>> GetCampaignContentsAsync(string campaignId)
        {
            throw new NotImplementedException();
        }

        public Task<PatientDashboardDTO> GetDashboardAsync(string patientCustomId, string campaignId)
        {
            throw new NotImplementedException();
        }

        public Task<PatientInvite> GetInviteAsync(string patientCustomId, string campaignId)
        {
            throw new NotImplementedException();
        }

        public Task<Patient> GetPatientByCustomIdAsync(string patientCustomId)
        {
            throw new NotImplementedException();
        }

        public Task RecordContentViewAsync(string patientId, string campaignId, string contentId)
        {
            throw new NotImplementedException();
        }

        // Send campaign to patients filtered by HCP specialty
        public async Task SendCampaignToPatientsAsync(string campaignId, string hcpId, string specialty)
        {
            var patients = await _patientRepo.GetPatientsBySpecialtyAsync(specialty);

            foreach (var patient in patients)
            {
                var invite = new PatientInvite
                {
                    CampaignId = campaignId,
                    HcpId = hcpId,
                    PatientId = patient.Id,
                    PatientCustomId = patient.PatientCustomId,
                    InviteId = Guid.NewGuid().ToString()
                };

                await _inviteRepo.CreateAsync(invite);

                var link = $"https://localhost:7286/campaigns/{campaignId}/view?inviteId={invite.InviteId}";

                await _emailService.SendEmailAsync(
                    patient.Email,
                    "New Campaign Available",
                    $"Hello {patient.Name},<br><br>You have a new healthcare campaign to view.<br><br><a href='{link}'>Click here to view</a>"
                );
            }
        }
    }
}
