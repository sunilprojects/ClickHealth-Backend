using ClickHealthBackend.DTOs;
using ClickHealthBackend.Models;

namespace ClickHealthBackend.Services.Interfaces
{
    public interface IPatientInviteService
    {
        Task<Patient> CreateOrGetPatientAsync(Patient patient);
        Task<PatientInvite> CreateInviteAsync(string patientId, string patientCustomId, string campaignId, string hcpId);

        Task<Patient> GetPatientByCustomIdAsync(string patientCustomId);
        Task<PatientInvite> GetInviteAsync(string patientCustomId, string campaignId);

        Task<List<Content>> GetCampaignContentsAsync(string campaignId);

        Task RecordContentViewAsync(string patientId, string campaignId, string contentId);

        Task<PatientDashboardDTO> GetDashboardAsync(string patientCustomId, string campaignId);
        Task SendCampaignToPatientsAsync(string campaignId, string hcpId, string specialty);

    }

}
