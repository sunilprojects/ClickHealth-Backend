using ClickHealthBackend.Models;
namespace ClickHealthBackend.Repositories.Interfaces
{
    public interface IHCPActivityRepository
    {
        Task CreateAsync(HCPActivity activity);
        Task<IEnumerable<HCPActivity>> GetAllAsync();
    }
}
