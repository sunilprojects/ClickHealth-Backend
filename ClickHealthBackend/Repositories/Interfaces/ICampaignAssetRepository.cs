using ClickHealthBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.Repositories.Interfaces
{
    public interface ICampaignAssetRepository
    {
        // New method to fetch all ContentIds linked to a list of CampaignIds efficiently
        Task<IEnumerable<string>> GetContentIdsByCampaignIdsAsync(List<string> campaignIds);

        // You would typically have CreateAsync, DeleteAsync here as well
    }
}