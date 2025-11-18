using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Data;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.Repositories.Implementations
{
    public class PatientEngagementRepository : IPatientEngagementRepository
    {
        private readonly IMongoCollection<PatientEngagement> _engagements;

        public PatientEngagementRepository(MongoDbContext context)
        {
            _engagements = context.PatientEngagement;
        }

        public async Task CreateAsync(PatientEngagement engagement) =>
            await _engagements.InsertOneAsync(engagement);

        public async Task<IEnumerable<PatientEngagement>> GetAllAsync() =>
            await _engagements.Find(_ => true).ToListAsync();
    }
}