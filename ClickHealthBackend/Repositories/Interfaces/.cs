using ClickHealthBackend.Models;

namespace ClickHealthBackend.Repositories.Interfaces
{
    public interface IPatientInviteRepository
    {
        Task CreateAsync(PatientInvite invite);
        public Task<PatientInvite> GetByPatientAndCampaignAsync(string patientCustomId, string campaignId);


    }

}
