using ClickHealthBackend.Data;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ClickHealthBackend.Repositories.Implementations
{
    public class PatientInviteRepository : IPatientInviteRepository
    {
        private readonly IMongoCollection<PatientInvite> _collection;

        public PatientInviteRepository(MongoDbContext context)
        {
            _collection = context.PatientInvites;
        }

        public async Task CreateAsync(PatientInvite invite)
        {
            if (string.IsNullOrEmpty(invite.Id))
                invite.Id = ObjectId.GenerateNewId().ToString();

            await _collection.InsertOneAsync(invite);
        }

        public async Task<List<PatientInvite>> GetByCampaignIdAsync(string campaignId)
        {
            return await _collection.Find(x => x.CampaignId == campaignId).ToListAsync();
        }

        // ✔ FIXED IMPLEMENTATION
        public async Task<PatientInvite> GetByPatientAndCampaignAsync(string patientCustomId, string campaignCustomId)
        {
            var filter = Builders<PatientInvite>.Filter.And(
                Builders<PatientInvite>.Filter.Eq(x => x.PatientCustomId, patientCustomId),
                Builders<PatientInvite>.Filter.Eq(x => x.CampaignId, campaignCustomId)
            );

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
