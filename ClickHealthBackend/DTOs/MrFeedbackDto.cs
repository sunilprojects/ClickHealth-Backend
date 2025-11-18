namespace ClickHealthBackend.DTOs
{
    public class MrFeedbackDto
    {
        public string MrUserId { get; set; }
        public string HcpUserId { get; set; }
        public string? CampaignId { get; set; }
        public string Notes { get; set; }
    }
}
