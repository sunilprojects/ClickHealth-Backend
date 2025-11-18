namespace ClickHealthBackend.DTOs
{
    public class DashboardDTO
    {
        public int ActiveCampaigns { get; set; }
        public string CampaignBreakdown { get; set; } = "";
        public int TotalReach { get; set; }
        public string ReachDetail { get; set; } = "";
        public int DoctorShares { get; set; }
        public string DoctorSharesDetail { get; set; } = "";
        public int CompletionRate { get; set; }
        public string CompletionRateDetail { get; set; } = "";
        //public List<ROIData> ROISignals { get; set; } = new();
        //public List<InsightData> Insights { get; set; } = new();
    }
}
