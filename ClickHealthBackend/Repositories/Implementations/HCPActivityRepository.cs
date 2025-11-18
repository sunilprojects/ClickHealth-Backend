using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Data; 
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.Repositories.Implementations
{
    public class HCPActivityRepository : IHCPActivityRepository
    {
        // Change: Field now holds the direct collection instance
        private readonly IMongoCollection<HCPActivity> _activities;

        // Change: Inject MongoDbContext instead of IMongoClient and databaseName
        public HCPActivityRepository(MongoDbContext context)
        {
            // Change: Get the collection directly from the MongoDbContext property
            _activities = context.HCPActivity;
        }

        public async Task CreateAsync(HCPActivity activity) =>
            await _activities.InsertOneAsync(activity);

        public async Task<IEnumerable<HCPActivity>> GetAllAsync() =>
            await _activities.Find(_ => true).ToListAsync();
    }
} 