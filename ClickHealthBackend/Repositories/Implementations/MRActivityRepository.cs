using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Data;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.Repositories.Implementations
{
    public class MRActivityRepository : IMRActivityRepository
    {
        private readonly IMongoCollection<MRActivity> _activities;

        public MRActivityRepository(MongoDbContext context)
        {
            _activities = context.MRActivity;
        }

        public async Task CreateAsync(MRActivity activity) =>
            await _activities.InsertOneAsync(activity);

        public async Task<IEnumerable<MRActivity>> GetAllAsync() =>
            await _activities.Find(_ => true).ToListAsync();
    }
}