namespace ClickHealthBackend.Models
{
    public class AdminDashboardSummary
    {
        public int TotalUsers { get; set; }
        public int PendingApprovals { get; set; }
        public int ActiveDoctors { get; set; }
        public List<UserSummary> RecentUsers { get; set; } = new();
    }

   

}
