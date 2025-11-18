using ClickHealthBackend.Enums;

namespace ClickHealthBackend.DTOs
{
    public class RegistrationRequestDto
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public UserRole Role { get; set; }
        public string Specialty { get; set; }
        public string Territory { get; set; }
        public string PreferredLanguage { get; set; }
    }
}