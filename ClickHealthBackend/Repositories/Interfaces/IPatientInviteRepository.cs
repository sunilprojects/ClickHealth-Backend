using ClickHealthBackend.Models;

namespace ClickHealthBackend.Repositories.Interface
{
    public interface IPatientInviteRepository
    {
        Task CreateAsync(PatientInvite invite);
        Task<PatientInvite> GetByPatientAndCampaignAsync(string pid, string cid);
        Task UpdateAsync(PatientInvite invite);
    }
}
