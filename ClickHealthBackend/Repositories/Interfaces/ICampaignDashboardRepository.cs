namespace ClickHealthBackend.Repositories.Interfaces
{
    public interface ICampaignDashboardRepository
    {
        Task<int> GetActiveCampaignsCountAsync();
        Task<Dictionary<string, int>> GetCampaignBreakdownAsync();
        Task<int> GetTotalReachAsync();
        Task<int> GetDoctorSharesAsync();
        Task<int> GetCompletionRateAsync();
    }
}
