using ClickHealthBackend.Data;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClickHealthBackend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // 1) Start Google Login
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth", null, Request.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // 2) Google Callback
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            // Authenticate using cookie scheme (used by Google OAuth)
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return Unauthorized("Google authentication failed.");

            var email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal?.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
                return BadRequest("Email not returned by Google.");

            // Check if user exists and is approved
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
                return BadRequest("Your account is not registered. Contact admin.");

            if (!user.IsApproved)
                return Unauthorized("Your account is not approved by admin.");

            // Redirect back to frontend (Blazor) with query parameters for auto-login
            var frontendUrl = $"https://localhost:7071/google-auth?email={email}&role={user.Role}";
            return Redirect(frontendUrl);
        }

        // 3) Auto-login from Blazor using JWT
        public class GoogleLoginDto
        {
            public string Email { get; set; } = string.Empty;
        }

        [HttpPost("google-auto-login")]
        public async Task<IActionResult> GoogleAutoLogin([FromBody] GoogleLoginDto dto)
        {
            var user = await _userService.GetByEmailAsync(dto.Email);
            if (user == null) return BadRequest("Your account is not registered. Contact admin.");

            var token = _userService.GenerateJwtToken(user);

            return Ok(new
            {
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = token
            });
        }

}
}
