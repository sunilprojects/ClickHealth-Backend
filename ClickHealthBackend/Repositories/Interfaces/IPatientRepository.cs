using ClickHealthBackend.Models;

namespace ClickHealthBackend.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task<string> GeneratePatientCustomIdAsync();
        Task<List<Patient>> GetPatientsBySpecialtyAsync(string specialty);

        Task CreateAsync(Patient patient);
        Task<Patient> GetByEmailAsync(string email);

        Task<Patient> GetByCustomIdAsync(string patientCustomId);



    }

}
