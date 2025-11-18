namespace ClickHealthBackend.DTOs
{
    public class HcpOnboardingRequestDto
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Specialty { get; set; }
        public string Territory { get; set; }
        public string MrUserId { get; set; }
    }
}
