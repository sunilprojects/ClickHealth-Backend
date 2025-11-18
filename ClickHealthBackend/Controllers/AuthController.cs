using ClickHealthBackend.Data;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClickHealthBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly MongoDbContext _context;
        private readonly IEmailService _emailService;
        private readonly JwtSettings _jwtSettings;

        public AuthController(MongoDbContext context, IEmailService emailService, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _emailService = emailService;
            _jwtSettings = jwtSettings.Value;
        }

        // --------------------------------------------------------------------
        // ✅ Manual Login (Email + Password)
        // --------------------------------------------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return Unauthorized(new { message = "Invalid credentials" });

            var token = GenerateJwtToken(user);
            return Ok(new
            {
                message = "Login successful",
                token,
                user = new { user.UserId, user.Email, user.Name, user.Role }
            });
        }

        // --------------------------------------------------------------------
        // ✅ Google Login Redirect
        // --------------------------------------------------------------------
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleCallback", "Auth", null, Request.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // --------------------------------------------------------------------
        // ✅ Google Callback (OAuth Response)
        // --------------------------------------------------------------------
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync();

            if (!result.Succeeded || result.Principal == null)
                return Redirect("/api/auth/error?reason=Google authentication failed");

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var name = result.Principal.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
                return Redirect("/api/auth/error?reason=Missing email claim from Google");

            var users = _context.Users;
            var existingUser = await users.Find(u => u.Email == email).FirstOrDefaultAsync();

            if (existingUser == null)
            {
                var newUser = new User
                {
                    Email = email,
                    Name = name ?? "New Google User",
                    Role = UserRole.Guest,
                    IsActive = true,
                    IsApproved = false,
                    CreatedAt = DateTime.UtcNow,
                    Password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString())
                };

                await users.InsertOneAsync(newUser);
                existingUser = newUser;
            }

            // Generate JWT token
            var jwtToken = GenerateJwtToken(existingUser);

            // Redirect to frontend with token
            string frontendUrl = $"https://localhost:7071/login-success?token={jwtToken}";
            return Redirect(frontendUrl);
        }

        // --------------------------------------------------------------------
        // ✅ Error Endpoint
        // --------------------------------------------------------------------
        [HttpGet("error")]
        public IActionResult Error(string reason = "Unknown error")
        {
            return BadRequest(new { message = $"Google login failed: {reason}" });
        }

        // --------------------------------------------------------------------
        // ✅ Generate JWT Token (for User)
        // --------------------------------------------------------------------
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId ?? Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim("role", user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // --------------------------------------------------------------------
        // ✅ Supporting DTO
        // --------------------------------------------------------------------
        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
