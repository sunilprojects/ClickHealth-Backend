using ClickHealthBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.Services.Interfaces
{
    public interface IHCPService
    {
        /// <summary>
        /// Retrieves campaign-based content assigned to an HCP by an MR.
        /// </summary>
        /// <param name="hcpUserId">The unique ID of the HCP.</param>
        /// <returns>A list of assigned content items.</returns>
        Task<IEnumerable<Content>> GetAssignedCampaignContentAsync(string hcpUserId);

        /// <summary>
        /// Generates a secure invite link for a patient tied to a specific campaign and sends the invitation via email.
        /// </summary>
        /// <param name="hcpUserId">The HCP sending the invite.</param>
        /// <param name="campaignId">The campaign the invite is associated with.</param>
        /// <param name="patientEmail">The email address of the patient.</param>
        /// <returns>A unique invite code (token) if successful; otherwise, null or empty.</returns>
        Task<string> GeneratePatientInviteLinkAsync(string hcpUserId, string campaignId, string patientEmail);

        /// <summary>
        /// Logs completion of a quiz by an HCP.
        /// </summary>
        /// <param name="hcpUserId">The HCP completing the quiz.</param>
        /// <param name="contentId">The ID of the content containing the quiz.</param>
        /// <param name="quizResponses">The user's submitted quiz answers.</param>
        /// <returns>True if the log was saved successfully; otherwise false.</returns>
        Task<bool> LogQuizCompletionAsync(string hcpUserId, string contentId, Dictionary<string, object> quizResponses);

        /// <summary>
        /// Retrieves a list of all approved content available in the system.
        /// </summary>
        /// <returns>A list of approved content items.</returns>
        Task<IEnumerable<Content>> GetApprovedContentAsync();
    }
}
