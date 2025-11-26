using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using MongoDB.Driver;

namespace ClickHealthBackend.Repositories.Implementations
{
    public class PatientRepository : IPatientRepository
    {
        private readonly IMongoCollection<Patient> _collection;

        public PatientRepository(IMongoDatabase db)
        {
            _collection = db.GetCollection<Patient>("Patients");
        }

        public async Task<string> GeneratePatientCustomIdAsync()
        {
            long count = await _collection.CountDocumentsAsync(_ => true);
            return $"PAT-{(count + 1).ToString("00000")}";
        }

        public async Task CreateAsync(Patient patient)
        {
            await _collection.InsertOneAsync(patient);
        }

        public async Task<Patient> GetByEmailAsync(string email)
        {
            return await _collection.Find(p => p.Email == email).FirstOrDefaultAsync();
        }

        public async Task<Patient> GetByCustomIdAsync(string patientCustomId)
        {
            return await _collection.Find(x => x.PatientCustomId == patientCustomId)
                                    .FirstOrDefaultAsync();
        }
        public async Task<List<Patient>> GetPatientsBySpecialtyAsync(string specialty)
        {
            return await _collection
                .Find(p => p.Specialty.ToLower() == specialty.ToLower())
                .ToListAsync();
        }




    }

}
