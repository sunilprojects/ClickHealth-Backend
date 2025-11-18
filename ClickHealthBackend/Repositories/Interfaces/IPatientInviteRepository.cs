using ClickHealthBackend.Models;

namespace ClickHealthBackend.Repositories.Interfaces
{
    public interface IPatientInviteRepository
    {
        Task CreateAsync(Patient invite);
        Task<Patient> GetByInviteCodeAsync(string inviteCode);
        Task<bool> UpdateAsync(Patient invite);
        Task<IEnumerable<Patient>> GetAllAsync();
    }
}
