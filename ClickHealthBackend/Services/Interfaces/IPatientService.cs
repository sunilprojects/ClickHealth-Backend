using ClickHealthBackend.Models;

namespace ClickHealthBackend.Services.Interfaces
{
    public interface IPatientService
    {
        Task<Content> GetContentByInviteCodeAsync(string inviteCode);

        // Includes IP address for compliance logging
        Task<bool> RecordPatientConsentAsync(string inviteCode, string userIpAddress);

        // Includes all engagement metrics: duration, location, and language
        Task LogContentEngagementAsync(string inviteCode, string engagementType, int durationSeconds, string city, string language);

        Task<bool> LogQuizCompletionAsync(string inviteCode, string contentId, Dictionary<string, object> quizResponses);
    }
}
