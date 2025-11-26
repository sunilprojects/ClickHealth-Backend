namespace ClickHealthBackend.DTOs
{
    public class HCPResponseDto
    {
        public string HcpId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Specialty { get; set; }
        public bool IsActive { get; set; }

        public string Email { get; set; }

    }
}
