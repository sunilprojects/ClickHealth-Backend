namespace ClickHealthBackend.DTOs
{
    public class TerritoryPerformanceDTO
    {
        public string Territory { get; set; }
        public double EngagementPercentage { get; set; }
        public int ActiveCampaignsCount { get; set; }
        public int HCPAmount { get; set; }
        public long PatientsReached { get; set; }
        public string OverallPerformance { get; set; } // e.g., "High", "Medium", "Low"
    }
}
