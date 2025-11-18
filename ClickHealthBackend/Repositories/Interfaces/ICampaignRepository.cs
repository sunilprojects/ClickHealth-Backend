using ClickHealthBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.Repositories.Interfaces
{
    public interface ICampaignRepository
    {
        Task<Campaign> GetLastCampaignAsync();

        Task<Campaign> CreateCampaignAsync(Campaign campaign);
        Task<List<Campaign>> GetAllCampaignsAsync();

        // Standardized method for retrieving by ID
        Task<Campaign> GetByIdAsync(string campaignId);

        Task CreateAsync(Campaign campaign);
        Task<bool> UpdateAsync(Campaign campaign);
        Task<bool> DeleteAsync(string campaignId);
        Task<Campaign> GetCampaignByIdAsync(string id);

        Task<string> GenerateCampaignCustomIdAsync();

    }
}