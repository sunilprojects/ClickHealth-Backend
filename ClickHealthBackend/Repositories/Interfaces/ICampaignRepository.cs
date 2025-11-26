using ClickHealthBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.Repositories.Interfaces
{
    public interface ICampaignRepository
    {
        Task<Campaign> CreateCampaignAsync(Campaign campaign);
        Task<List<Campaign>> GetAllCampaignsAsync();

        // The only ID we use = CampaignCustomId
        Task<Campaign> GetCampaignByIdAsync(string campaignCustomId);

        Task<bool> UpdateAsync(Campaign campaign);
        Task<bool> DeleteAsync(string campaignCustomId);
        Task<Campaign> GetByIdAsync(string campaignId);

        Task<string> GenerateCampaignCustomIdAsync();
        Task<Campaign> GetLastCampaignAsync();
    }
}
