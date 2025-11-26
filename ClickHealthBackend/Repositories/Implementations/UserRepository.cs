using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UserRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("ClickHealthDb");
            _usersCollection = database.GetCollection<User>("Users");
        }

        public async Task<User?> GetByIdAsync(string id) =>
            await _usersCollection.Find(u => u.UserId == id).FirstOrDefaultAsync();

        public async Task<User?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            return await _usersCollection.Find(u => u.Email.ToLower() == email.ToLower()).FirstOrDefaultAsync();
        }
        public async Task<List<User>> GetPendingUsersAsync() =>
            await _usersCollection.Find(u => u.IsApproved == false).ToListAsync();

        public async Task CreateAsync(User user) =>
            await _usersCollection.InsertOneAsync(user);

        public async Task<bool> UpdateAsync(string id, User user)
        {
            var result = await _usersCollection.ReplaceOneAsync(u => u.UserId == id, user);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> ExistsAsync(string email) =>
            await _usersCollection.Find(u => u.Email == email).AnyAsync();

        public async Task<bool> ApproveUserAsync(string email, string hashedPassword)
        {
            var update = Builders<User>.Update
                .Set(u => u.Password, hashedPassword)
                .Set(u => u.IsApproved, true);

            var result = await _usersCollection.UpdateOneAsync(u => u.Email == email, update);
            return result.ModifiedCount > 0;
        }

        public async Task<List<User>> GetUsersByStatusAsync(UserStatus status)
        {
            return await _usersCollection
                .Find(u => u.Status == status)
                .ToListAsync();
        }

        public async Task<List<User>> GetAllAsync() =>
            await _usersCollection.Find(_ => true).ToListAsync();

        public async Task<string> GenerateCustomIdAsync(string prefix)
        {
            var sort = Builders<User>.Sort.Descending(x => x.UserCustomId);

            var lastUser = await _usersCollection
                .Find(u => u.UserCustomId.StartsWith(prefix))
                .Sort(sort)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastUser != null && !string.IsNullOrEmpty(lastUser.UserCustomId))
            {
                string numberPart = new string(lastUser.UserCustomId
                    .Skip(prefix.Length)
                    .Where(char.IsDigit)
                    .ToArray());

                if (int.TryParse(numberPart, out int numericValue))
                {
                    nextNumber = numericValue + 1;
                }
            }

            return $"{prefix}{nextNumber:D3}";
        }

     
    }
}
