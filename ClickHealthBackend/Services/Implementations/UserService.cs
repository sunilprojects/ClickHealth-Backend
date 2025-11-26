using ClickHealthBackend.DTOs;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClickHealthBackend.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IEmailService _emailService;
        private readonly JwtSettings _jwtSettings; // <-- Injected JWT config
        private readonly int otpValidityMinutes = 3;

        private readonly IConfiguration _configuration;

        public UserService(
     IUserRepository repo,
     IEmailService emailService,
     IOptions<JwtSettings> jwtSettings,
     IConfiguration configuration)
        {
            _repo = repo;
            _emailService = emailService;
            _jwtSettings = jwtSettings.Value;
            _configuration = configuration;
        }
        // -------------------------------
        // LOGIN USING PASSWORD
        // -------------------------------
        public async Task<User?> UserLoginAsync(string email, string password)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null || !user.IsApproved) return null;
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password)) return null;

            if (user.MustResetPassword) return user;

            user.LastLoginAt = DateTime.UtcNow;
            await _repo.UpdateAsync(user.UserId, user);

            return user;
        }

        // -------------------------------
        // LOGIN USING OTP
        // -------------------------------
        public async Task<User?> UserLoginWithOtpAsync(string email, string otp)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null || !user.IsApproved || string.IsNullOrEmpty(user.Totp)) return null;

            if (user.TotpGeneratedAt == null ||
                (DateTime.UtcNow - user.TotpGeneratedAt.Value).TotalMinutes > otpValidityMinutes)
                return null;

            if (user.Totp != otp) return null;

            user.Totp = null;
            user.TotpGeneratedAt = null;
            user.LastLoginAt = DateTime.UtcNow;
            await _repo.UpdateAsync(user.UserId, user);

            return user;
        }

        // -------------------------------
        // SEND OTP
        // -------------------------------
        public async Task<bool> SendOtpAsync(string email)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null || !user.IsApproved) return false;

            var otp = new Random().Next(100000, 999999).ToString();
            user.Totp = otp;
            user.TotpGeneratedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(user.UserId, user);

            await _emailService.SendEmailAsync(
                user.Email,
                "ClickHealth OTP",
                $"Your OTP is <b>{otp}</b>. It expires in {otpValidityMinutes} minutes."
            );

            return true;
        }

        // -------------------------------
        // VERIFY OTP (LOGIN)
        // -------------------------------
        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null || user.Totp == null) return false;

            if (user.TotpGeneratedAt == null ||
                (DateTime.UtcNow - user.TotpGeneratedAt.Value).TotalMinutes > otpValidityMinutes)
                return false;

            if (user.Totp != otp) return false;

            user.Totp = null;
            user.TotpGeneratedAt = null;
            await _repo.UpdateAsync(user.UserId, user);

            return true;
        }

        // -------------------------------
        // VERIFY OTP FOR RESET
        // -------------------------------
        public async Task<bool> VerifyOtpForResetAsync(string email, string otp)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null || user.Totp == null) return false;

            if (user.TotpGeneratedAt == null ||
                (DateTime.UtcNow - user.TotpGeneratedAt.Value).TotalMinutes > otpValidityMinutes)
                return false;

            if (user.Totp != otp) return false;

            user.Totp = null;
            user.TotpGeneratedAt = null;
            await _repo.UpdateAsync(user.UserId, user);

            return true;
        }

        // -------------------------------
        // UPDATE PASSWORD
        // -------------------------------
        public async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null) return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.MustResetPassword = false;
            user.LastPasswordChangeAt = DateTime.UtcNow;

            await _repo.UpdateAsync(user.UserId, user);
            return true;
        }

        // -------------------------------
        // REGISTRATION
        // -------------------------------
        public async Task<User> RegisterUserAsync(RegistrationRequestDto request)
        {
            if (await _repo.ExistsAsync(request.Email))
                throw new Exception("Email already exists");

            var newUser = new User
            {
                Email = request.Email,
                Name = request.Name,
                Role = request.Role,
                Phone = request.Phone,
                Specialty = request.Specialty,
                Territory = request.Territory,
                PreferredLanguage = request.PreferredLanguage,
                IsApproved = false,
                IsActive = true,
                Status = UserStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.CreateAsync(newUser);
            return newUser;
        }

        // -------------------------------
        // ADMIN APPROVE USER
        // -------------------------------
        public async Task<bool> ApproveUserAsync(string email, UserRole role)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null || user.Status != UserStatus.Pending)
                return false;

            // Generate Custom ID for Medical Users ONLY on Approval
            if (role == UserRole.Medical_Team)
            {
                user.UserCustomId = await _repo.GenerateCustomIdAsync("M");
            }
            else if (role == UserRole.Marketing_Team)
            {
                user.UserCustomId = await _repo.GenerateCustomIdAsync("MT");
            }
            else if (role == UserRole.HCP)
            {
                user.UserCustomId = await _repo.GenerateCustomIdAsync("HCP"); ;
            }

            var tempPassword = Guid.NewGuid().ToString().Substring(0, 8);
            user.Password = BCrypt.Net.BCrypt.HashPassword(tempPassword);
            user.Role = role;
            user.IsApproved = true;
            user.MustResetPassword = true;
            user.Status = UserStatus.Approved;

            await _repo.UpdateAsync(user.UserId, user);

            await _emailService.SendEmailAsync(
                user.Email,
                "ClickHealth Account Approved",
                $"Welcome! Temporary password: <b>{tempPassword}</b>. Please reset after first login."
            );

            return true;
        }

        // -------------------------------
        // ADMIN REJECT USER
        // -------------------------------
        public async Task<bool> RejectUserAsync(string email, string reason = null)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null || user.Status != UserStatus.Pending) return false;

            user.Status = UserStatus.Rejected;
            await _repo.UpdateAsync(user.UserId, user);

            await _emailService.SendEmailAsync(
                user.Email,
                "ClickHealth Account Rejected",
                $"Your account registration has been rejected.{(string.IsNullOrEmpty(reason) ? "" : $" Reason: {reason}")}"
            );

            return true;
        }

        // -------------------------------
        // GET USERS BY STATUS
        // -------------------------------
        public async Task<List<User>> GetUsersByStatusAsync(UserStatus status) =>
            await _repo.GetUsersByStatusAsync(status);

        public async Task<List<User>> GetPendingUsersAsync() =>
            await _repo.GetUsersByStatusAsync(UserStatus.Pending);

        public async Task<User?> AdminLoginAsync(string email, string password)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null) return null;

            if (user.Role != UserRole.Admin) return null;
            if (!user.IsApproved || !user.IsActive) return null;
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password)) return null;

            return user;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _repo.GetAllAsync();
        }

        // -------------------------------
        // GENERATE JWT TOKEN
        // -------------------------------
        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("UserId", user.UserId)
        }),
                Expires = DateTime.UtcNow.AddHours(12),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _repo.GetByEmailAsync(email);
        }


    }
}
