namespace ClickHealthBackend.DTOs
{
    public class ShareContentDto
    {
        public string PatientId { get; set; }
        public string HcpUserId { get; set; }
        public List<string> ContentIds { get; set; }
        public string CampaignId { get; set; }
        public string ShareChannel { get; set; } 
    }
}
