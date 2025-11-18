using ClickHealthBackend.Models;
namespace ClickHealthBackend.Repositories.Interfaces
{
    public interface IMRActivityRepository
    {
        Task CreateAsync(MRActivity activity);

        Task<IEnumerable<MRActivity>> GetAllAsync(); // ADDED for real-time reporting/dashboards
    }
}
