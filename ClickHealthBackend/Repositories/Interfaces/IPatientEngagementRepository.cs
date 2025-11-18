using ClickHealthBackend.Models;

namespace ClickHealthBackend.Repositories.Interfaces
{
    public interface IPatientEngagementRepository
    {
        Task CreateAsync(PatientEngagement engagement);
        Task<IEnumerable<PatientEngagement>> GetAllAsync();
    }
}
