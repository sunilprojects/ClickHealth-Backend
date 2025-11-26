
using ClickHealthBackend.Models;

namespace ClickHealthBackend.Repositories.Interfaces
{
    public interface IHCPRepository
    {

        Task<HCP> GetHCPByIdAsync(string hcpId);
        Task<List<HCP>> GetAllHCPsAsync();
        Task<HCP> CreateHCPAsync(HCP hcp);
    }
}
