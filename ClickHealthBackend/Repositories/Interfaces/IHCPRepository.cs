using ClickHealthBackend.Models;

namespace ClickHealthBackend.Repositories.Interfaces
{
    public interface IHCPRepository
    {
        // For Total HCP Onboarded
        Task<long> GetTotalHCPOnboardedAsync();

        // For Most Engaged HCPs and Needs Attention (Lowest Engaged)
        Task<List<User>> GetEngagedHCPsAsync(int limit = 10, bool ascending = false); // ascending=true for lowest engaged

        // For Monthly Active Users (HCPs)
        Task<long> GetMonthlyActiveHCPCountAsync();
    }
}
