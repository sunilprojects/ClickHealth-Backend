namespace ClickHealthBackend.DTOs
{
    public class PatientInviteRequestDto
    {
        public string RecipientEmail { get; set; }
        public string ContentId { get; set; }
        public string? CampaignId { get; set; }
    }
}
