using ClickHealthBackend.DTOs;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var admin = await _userService.AdminLoginAsync(dto.Email, dto.Password);
        if (admin == null) return Unauthorized("Invalid admin credentials");

        return Ok(new { Message = "Admin login successful" });
    }

    [HttpGet("pending-users")]
    public async Task<IActionResult> GetPendingUsers()
    {
        var users = await _userService.GetPendingUsersAsync();
        return Ok(users);
    }

    [HttpPost("approve-user/{email}")]
    public async Task<IActionResult> ApproveUser(string email, [FromBody] UserRole role)
    {
        var success = await _userService.ApproveUserAsync(email, role);
        if (!success) return NotFound("User not found or not pending");
        return Ok("User approved successfully");
    }

    [HttpPost("reject-user/{email}")]
    public async Task<IActionResult> RejectUser(string email, [FromBody] string reason = "")
    {
        var success = await _userService.RejectUserAsync(email, reason);
        if (!success) return NotFound("User not found or not pending");
        return Ok("User rejected successfully");
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();

        if (users == null || users.Count == 0)
            return NotFound("No users found.");

        return Ok(users);
    }


    [HttpGet("dashboard-summary")]
    public async Task<ActionResult<AdminDashboardSummary>> GetDashboardSummary()
    {
        var users = await _userService.GetAllUsersAsync();

        var summary = new AdminDashboardSummary
        {
            TotalUsers = users.Count,
            PendingApprovals = users.Count(u => !u.IsApproved),
            ActiveDoctors = users.Count(u => u.Role == UserRole.HCP && u.IsApproved),
            RecentUsers = users
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .Select(u => new UserSummary
                {
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role.ToString(),
                    IsApproved = u.IsApproved
                }).ToList()
        };

        return Ok(summary);
    }

}
