using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(string id);
        Task<List<User>> GetPendingUsersAsync();
        Task CreateAsync(User user);
        Task<bool> UpdateAsync(string id, User user);
        Task<bool> ExistsAsync(string email);

        Task<List<User>> GetUsersByStatusAsync(UserStatus status);
        Task<List<User>> GetAllAsync();
        
        Task<string> GenerateCustomIdAsync(String prefix);
    }
}
