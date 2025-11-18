using ClickHealthBackend.Enums;

namespace ClickHealthBackend.DTOs
{
    public class LoginResponseDto
    {
        public string Email { get; set; } = null!;
        public UserRole Role { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
