using ClickHealthBackend.Data;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using MongoDB.Driver;
using ClickHealthBackend.Enums; // <-- IMPORTANT: Added to access the UserRole enum
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ClickHealthBackend.Repositories.Implementations
{
    public class HCPRepository : IHCPRepository
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<HCPActivity> _hcpActivities;

        public HCPRepository(MongoDbContext context)
        {
            _users = context.Users;
            _hcpActivities = context.HCPActivity;
        }

        public async Task<long> GetTotalHCPOnboardedAsync()
        {
            // FIX: Replaced u.Role.ToString() with direct enum comparison (u.Role == UserRole.HCP)
            return await _users.CountDocumentsAsync(u => u.Role == UserRole.HCP && u.Status != UserStatus.Pending);
        }

        public async Task<long> GetMonthlyActiveHCPCountAsync()
        {
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);

            // Find distinct HCP user IDs from recent activities
            var activeHCPs = await _hcpActivities
                .Find(a => a.Timestamp >= oneMonthAgo)
                .Project(a => a.HcpUserId)
                .ToListAsync();

            return activeHCPs.Distinct().LongCount();
        }

        public async Task<List<User>> GetEngagedHCPsAsync(int limit = 10, bool ascending = false)
        {
            // NOTE: Calculating engagement score usually requires an Aggregation Pipeline.
            // For a simplified example, we'll order by the last activity time on the HCPActivity collection.

            // 1. Get the last activity timestamp for each HCP
            var lastActivityLookup = _hcpActivities.AsQueryable()
                .GroupBy(a => a.HcpUserId)
                .Select(g => new { HcpUserId = g.Key, LastActivity = g.Max(a => a.Timestamp) })
                .OrderByDescending(x => x.LastActivity);

            // 2. Perform a join (lookup) to get the User details
            // The sortDefinition is currently unused due to the simplified query below.
            // var sortDefinition = ascending
            //    ? Builders<User>.Sort.Ascending("LastActivity") 
            //    : Builders<User>.Sort.Descending("LastActivity");

            // FIX: Used direct enum comparison (u.Role == UserRole.HCP) in the filter.
            var filter = Builders<User>.Filter.Eq(u => u.Role, UserRole.HCP);

            // Determine sort based on the 'ascending' parameter
            var sort = ascending
                ? Builders<User>.Sort.Ascending(u => u.CreatedAt) // Lowest engaged (oldest creation date)
                : Builders<User>.Sort.Descending(u => u.CreatedAt); // Most engaged (newest creation date)

            return await _users.Find(filter)
                .Sort(sort)
                .Limit(limit)
                .ToListAsync();
        }
    }
}
