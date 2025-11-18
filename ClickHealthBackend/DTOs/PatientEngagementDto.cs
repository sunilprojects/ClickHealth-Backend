namespace ClickHealthBackend.DTOs
{
    public class PatientEngagementDto
    {
        public string InviteCode { get; set; }
        public int DurationSeconds { get; set; }
        public string City { get; set; }
        public string Language { get; set; }
        public string EngagementType { get; set; } // Must match enum string (View, Share, etc.)
    }
}
