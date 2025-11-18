namespace ClickHealthBackend.DTOs
{
    public class ContentShareDto
    {
        public string MrUserId { get; set; }

        // The HCP who receives the assignment
        public string HcpUserId { get; set; }

        // CRITICAL FIX: The unit of sharing is the Campaign.
        public string CampaignId { get; set; }
    }
}
