using ClickHealthBackend.DTOs;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.Services.Interfaces
{
    public interface IUserService
    {
        // Admin login
        Task<User?> AdminLoginAsync(string email, string password);

        // User login
        Task<User?> UserLoginAsync(string email, string password);          // Email + password
        Task<User?> UserLoginWithOtpAsync(string email, string otp);        // Email + OTP

        // Registration
        Task<User> RegisterUserAsync(RegistrationRequestDto request);

        // Pending approvals
        Task<List<User>> GetPendingUsersAsync();
        Task<List<User>> GetUsersByStatusAsync(UserStatus status);

        // Approve/Reject users
        Task<bool> ApproveUserAsync(string email, UserRole role);
        Task<bool> RejectUserAsync(string email, string reason = null);

        // OTP methods (users + admins)
        Task<bool> SendOtpAsync(string email);                               // For login or forgot password
        Task<bool> VerifyOtpAsync(string email, string otp);                // For login via OTP
        Task<bool> VerifyOtpForResetAsync(string email, string otp);        // For forgot password

        // Password update
        Task<bool> UpdatePasswordAsync(string email, string newPassword);  // First-time reset or forgot password

        Task<List<User>> GetAllUsersAsync();

        string GenerateJwtToken(User user);
    }
}
