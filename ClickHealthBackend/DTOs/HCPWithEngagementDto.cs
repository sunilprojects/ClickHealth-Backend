namespace ClickHealthBackend.Models
{
    public class HCPWithEngagementDto
    {
        public string HcpId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Specialty { get; set; }
        public long InviteCount { get; set; }
    }
}
