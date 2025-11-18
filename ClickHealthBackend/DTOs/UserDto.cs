using ClickHealthBackend.Enums;

namespace ClickHealthBackend.DTOs
{
    public class UserDto
    {
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public UserRole Role { get; set; }
        public string Specialty { get; set; } = null!;
        public string Territory { get; set; } = null!;
        public bool IsActive { get; set; }
        public string PreferredLanguage { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}

