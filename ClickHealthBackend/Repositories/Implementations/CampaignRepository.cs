using ClickHealthBackend.Data;

using ClickHealthBackend.Models;

using ClickHealthBackend.Repositories.Interfaces;

using MongoDB.Driver;

using System.Collections.Generic;

using System.Threading.Tasks;

namespace ClickHealthBackend.Repositories.Implementations

{

    public class CampaignRepository : ICampaignRepository

    {

        private readonly IMongoCollection<Campaign> _campaigns;

        public CampaignRepository(MongoDbContext context)

        {

            _campaigns = context.Campaigns;

        }

        // Create a new campaign

        public async Task<Campaign> CreateCampaignAsync(Campaign campaign)

        {

            await _campaigns.InsertOneAsync(campaign);

            return campaign;

        }

        // Get all campaigns

        public async Task<List<Campaign>> GetAllCampaignsAsync()

        {

            return await _campaigns.Find(_ => true).ToListAsync();

        }

        // Get campaign by ID

        public async Task<Campaign> GetByIdAsync(string campaignId)

        {

            return await _campaigns.Find(c => c.CampaignId == campaignId).FirstOrDefaultAsync();

        }

        // Update campaign

        public async Task<bool> UpdateAsync(Campaign campaign)

        {

            var result = await _campaigns.ReplaceOneAsync(c => c.CampaignId == campaign.CampaignId, campaign);

            return result.IsAcknowledged && result.ModifiedCount > 0;

        }

        // Delete campaign

        public async Task<bool> DeleteAsync(string campaignId)

        {

            var result = await _campaigns.DeleteOneAsync(c => c.CampaignId == campaignId);

            return result.IsAcknowledged && result.DeletedCount > 0;

        }

        public Task CreateAsync(Campaign campaign)

        {

            throw new NotImplementedException();

        }

        public Task<Campaign> GetCampaignByIdAsync(string id)
                    {

            throw new NotImplementedException();

        }

        public async Task<string> GenerateCampaignCustomIdAsync()
        {
            var sort = Builders<Campaign>.Sort.Descending(x => x.CampaignCustomId);

            var lastCampaign = await _campaigns
                .Find(_ => true)
                .Sort(sort)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastCampaign != null && !string.IsNullOrEmpty(lastCampaign.CampaignCustomId))
            {
                string numberPart = lastCampaign.CampaignCustomId.Replace("CMP", "");
                nextNumber = int.Parse(numberPart) + 1;
            }

            return $"CMP{nextNumber:D3}";
        }

        public Task<Campaign> GetLastCampaignAsync()
        {
            throw new NotImplementedException();
        }
    }

}

