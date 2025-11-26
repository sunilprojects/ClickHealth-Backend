using ClickHealthBackend.Data;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.Repositories.Implementations
{
    public class HCPRepository : IHCPRepository
    {
        private readonly IMongoCollection<HCP> _hcps;

        public HCPRepository(MongoDbContext context)
        {
            _hcps = context.HCPs;
        }

        public async Task<HCP> GetHCPByIdAsync(string hcpId)
        {
            return await _hcps.Find(h => h.HcpId == hcpId).FirstOrDefaultAsync();
        }

        public async Task<List<HCP>> GetAllHCPsAsync()
        {
            return await _hcps.Find(_ => true).ToListAsync();
        }

        public async Task<HCP> CreateHCPAsync(HCP hcp)
        {
            await _hcps.InsertOneAsync(hcp);
            return hcp;
        }
    }
}
