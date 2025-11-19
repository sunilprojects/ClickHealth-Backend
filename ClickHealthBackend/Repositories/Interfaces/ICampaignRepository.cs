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
<<<<<<< HEAD

=======
       
>>>>>>> 6d54bde216ffe9ad760fc6fd5b3df6d9b1538c81
    }
}