namespace ClickHealthBackend.DTOs
{
    // This DTO is used for returning data in GET endpoints
    public class CampaignDTO
    {
        public string CampaignId { get; set; }

        public string CampaignCustomId { get; set; }
        public string Name { get; set; }
        public string Therapy { get; set; }
        public List<string> Cities { get; set; }
        public List<string> Territories { get; set; }
        public string Language { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        // CRITICAL FIX: Use a simple C# Dictionary instead of BsonDocument
        public Dictionary<string, object> TargetMetrics { get; set; }
    }
}
