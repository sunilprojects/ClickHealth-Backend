using ClickHealth.Server.Models;
using ClickHealthBackend.Models;

namespace ClickHealthBackend.Repositories.Interfaces
{
    public interface IConsentRecordRepository
    {
        /// <summary>
        /// Creates a new ConsentRecord document in the database.
        /// </summary>
        Task CreateAsync(ConsentRecord record);
    }
}
