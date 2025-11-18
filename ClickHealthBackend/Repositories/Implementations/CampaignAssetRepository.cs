using MongoDB.Driver;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClickHealthBackend.Repositories.Implementations
{
    public class CampaignAssetRepository : ICampaignAssetRepository
    {
        private readonly IMongoCollection<CampaignAsset> _assets;

        public CampaignAssetRepository(MongoDbContext context)
        {
            // FIX: Changed access from the incorrect plural 'context.CampaignAssets'
            // to the correct singular property name 'context.CampaignAsset'.
            _assets = context.CampaignAsset;
        }

        public async Task<IEnumerable<string>> GetContentIdsByCampaignIdsAsync(List<string> campaignIds)
        {
            // The rest of the logic remains sound: filtering in the database
            var filter = Builders<CampaignAsset>.Filter.In(a => a.CampaignId, campaignIds);

            var contentIds = await _assets
                .Find(filter)
                .Project(a => a.ContentId)
                .ToListAsync();

            return contentIds.Distinct();
        }
    }
}