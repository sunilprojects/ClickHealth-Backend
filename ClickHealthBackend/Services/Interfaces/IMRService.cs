namespace ClickHealthBackend.Services.Interfaces
{
    public interface IMRService
    {
        Task<string> OnboardHCPAsync(string mrUserId, string hcpName, string hcpEmail, string hcpPhone, string territory, string specialty);
        Task<bool> ShareContentWithHCPAsync(string mrUserId, string hcpUserId, string contentId);
        Task LogFieldFeedbackAsync(string mrUserId, string notes);
        Task<object> GetTerritorySnapshotAsync(string mrUserId);
    }
}
