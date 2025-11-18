using ClickHealthBackend.DTOs;
using ClickHealthBackend.Models;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequestDto dto)
    {
        var user = await _userService.RegisterUserAsync(dto);
        return Ok(new { Message = "Registration successful. Wait for admin approval." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var user = await _userService.UserLoginAsync(dto.Email, dto.Password);
        if (user == null)
            return Unauthorized("Invalid credentials or not approved");

        if (user.MustResetPassword)
            return Ok(new { Message = "First-time login. Please reset your password." });

        var token = _userService.GenerateJwtToken(user);
        return Ok(new { Token = token, Role = user.Role, Email = user.Email });
    }


    [HttpPost("login-otp")]
    public async Task<IActionResult> LoginWithOtp([FromBody] VerifyOtpRequestDto dto)
    {
        var user = await _userService.UserLoginWithOtpAsync(dto.Email, dto.Otp);
        if (user == null) return Unauthorized("Invalid or expired OTP");

        return Ok(new { Message = "Login successful", Email = user.Email, Role = user.Role });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
    {
        var sent = await _userService.SendOtpAsync(dto.Email);
        if (!sent) return NotFound("Email not found or not approved.");
        return Ok("OTP sent to your email for password reset.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
    {
        var otpValid = await _userService.VerifyOtpForResetAsync(dto.Email, dto.Otp);
        if (!otpValid) return Unauthorized("Invalid or expired OTP.");

        var updated = await _userService.UpdatePasswordAsync(dto.Email, dto.NewPassword);
        if (!updated) return BadRequest("Failed to reset password.");

        return Ok("Password reset successfully. Please login again.");
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequestDto dto)
    {
        var sent = await _userService.SendOtpAsync(dto.Email);
        if (!sent) return NotFound("Email not found or not approved.");
        return Ok("OTP sent to your email.");
    }
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDto dto)
    {
        var user = await _userService.UserLoginWithOtpAsync(dto.Email, dto.Otp);
        if (user == null) return Unauthorized("Invalid or expired OTP");

        return Ok(new { Message = "Login successful", Email = user.Email, Role = user.Role });
    }

}
