namespace ClickHealthBackend.DTOs
{
    public class CreateCampaignDTO
    {
        // Only include fields needed for POST/Creation
        public string Name { get; set; }
        public string Therapy { get; set; }
        public List<string> Cities { get; set; } = new();
        public List<string> Territories { get; set; } = new();
        public string Language { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        // For target metrics, use a simple Dictionary<string, object> or a simpler DTO
        public Dictionary<string, object> TargetMetrics { get; set; }

        // DO NOT include: CampaignId, CreatedByUserId, CreatedAt (these are set by the server)
    }
}
