using Microsoft.AspNetCore.Mvc;
using ClickHealthBackend.Services.Interfaces;
using ClickHealthBackend.Models;
using ClickHealthBackend.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HCPController : ControllerBase
    {
        private readonly IHCPService _hcpService;

        public HCPController(IHCPService hcpService)
        {
            _hcpService = hcpService;
        }

        /// <summary>
        /// Retrieves all content (videos, PDFs, quizzes) assigned to an HCP by an MR.
        /// </summary>
        [HttpGet("content/assigned")]
        public async Task<IActionResult> GetAssignedContent([FromQuery] string hcpUserId)
        {
            if (string.IsNullOrWhiteSpace(hcpUserId))
                return BadRequest(new { error = "HCP user ID is required." });

            var contentList = await _hcpService.GetAssignedCampaignContentAsync(hcpUserId);

            if (contentList == null || !contentList.Any())
                return NotFound(new { message = "No content assigned to this HCP." });

            return Ok(contentList);
        }

        /// <summary>
        /// Generates a secure invite link for a patient tied to a specific campaign.
        /// </summary>
        [HttpPost("patient-invites")]
        public async Task<IActionResult> GeneratePatientInvite([FromBody] PatientInviteDto inviteDto)
        {
            if (inviteDto == null)
                return BadRequest(new { error = "Request body cannot be empty." });

            if (string.IsNullOrWhiteSpace(inviteDto.HcpUserId) ||
                string.IsNullOrWhiteSpace(inviteDto.CampaignId) ||
                string.IsNullOrWhiteSpace(inviteDto.PatientEmail))
            {
                return BadRequest(new { error = "HcpUserId, CampaignId, and PatientEmail are required." });
            }

            var inviteCode = await _hcpService.GeneratePatientInviteLinkAsync(
                inviteDto.HcpUserId,
                inviteDto.CampaignId,
                inviteDto.PatientEmail);

            if (string.IsNullOrWhiteSpace(inviteCode))
                return BadRequest(new { error = "Failed to generate invite link. The campaign might be inactive." });

            return Ok(new
            {
                message = "Secure invite link generated and emailed to patient.",
                inviteCode
            });
        }

        /// <summary>
        /// Logs completion of an optional quiz by the HCP.
        /// </summary>
        [HttpPost("quizzes/{contentId}/complete")]
        public async Task<IActionResult> CompleteQuiz(
            string contentId,
            [FromBody] QuizCompletionDto quizCompletionDto)
        {
            if (string.IsNullOrWhiteSpace(contentId))
                return BadRequest(new { error = "ContentId is required." });

            if (quizCompletionDto == null)
                return BadRequest(new { error = "Request body is required." });

            if (string.IsNullOrWhiteSpace(quizCompletionDto.HcpUserId))
                return BadRequest(new { error = "HcpUserId is required." });

            var success = await _hcpService.LogQuizCompletionAsync(
                quizCompletionDto.HcpUserId,
                contentId,
                quizCompletionDto.QuizResponses);

            if (!success)
                return BadRequest(new { error = "Failed to log quiz completion." });

            return Ok(new { message = "Quiz completion logged successfully." });
        }
    }
}
