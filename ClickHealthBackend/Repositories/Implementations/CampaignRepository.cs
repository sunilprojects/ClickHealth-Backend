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

        public async Task<Campaign> CreateCampaignAsync(Campaign campaign)
        {
            await _campaigns.InsertOneAsync(campaign);
            return campaign;
        }

        public async Task<List<Campaign>> GetAllCampaignsAsync()
        {
            return await _campaigns.Find(_ => true).ToListAsync();
        }

        // --- Fetch a campaign by CampaignCustomId ---
        public async Task<Campaign> GetCampaignByIdAsync(string campaignCustomId)
        {
            return await _campaigns
                .Find(c => c.CampaignCustomId == campaignCustomId)
                .FirstOrDefaultAsync();
        }

        // --- Update using CampaignCustomId ---
        public async Task<bool> UpdateAsync(Campaign campaign)
        {
            var result = await _campaigns.ReplaceOneAsync(
                c => c.CampaignCustomId == campaign.CampaignCustomId,
                campaign
            );

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        // --- Delete using CampaignCustomId ---
        public async Task<bool> DeleteAsync(string campaignCustomId)
        {
            var result = await _campaigns.DeleteOneAsync(
                c => c.CampaignCustomId == campaignCustomId
            );

            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        // --- Generate Campaign Custom ID (CMP001, CMP002...) ---
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

        public async Task<Campaign> GetLastCampaignAsync()
        {
            var sort = Builders<Campaign>.Sort.Descending(c => c.CampaignCustomId);

            return await _campaigns
                .Find(_ => true)
                .Sort(sort)
                .FirstOrDefaultAsync();
        }

        public async Task<Campaign> GetByIdAsync(string campaignCustomId)
        {
            return await _campaigns
                .Find(x => x.CampaignCustomId == campaignCustomId)
                .FirstOrDefaultAsync();
        }


    }
}
