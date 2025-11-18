using ClickHealth.Server.Models;
using ClickHealthBackend.Data;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace ClickHealthBackend.Repositories.Implementations
{
    public class ConsentRecordRepository : IConsentRecordRepository
    {
        private readonly IMongoCollection<ConsentRecord> _consentRecords;

        public ConsentRecordRepository(MongoDbContext context)
        {
            // Initializes the collection using the strongly typed property from MongoDbContext
            _consentRecords = context.ConsentRecord;
        }

        public async Task CreateAsync(ConsentRecord record)
        {
            // Inserts the new consent record into the database.
            await _consentRecords.InsertOneAsync(record);
        }
    }
}