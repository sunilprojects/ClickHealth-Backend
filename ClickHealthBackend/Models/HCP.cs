namespace ClickHealthBackend.Models
{
    public class HCP
    {
        public string HcpId { get; set; }  // Unique HCP Code like "HCP001"

        public string Name { get; set; }
        public string Specialty { get; set; }
        public string City { get; set; }
        public string Territory { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Engagement
        public int TotalEngagementScore { get; set; }
        public int MonthlyEngagementScore { get; set; }

        public string PhoneNumber { get; set; }  // ✅ Add this
    }
}
