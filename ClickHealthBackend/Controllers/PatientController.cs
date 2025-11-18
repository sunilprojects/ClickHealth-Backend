using ClickHealthBackend.DTOs;
using ClickHealthBackend.Models;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClickHealthBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        /// <summary>
        /// Retrieves the content (Video/PDF/Quiz details) based on the secure invite code.
        /// </summary>
        [HttpGet("content")]
        public async Task<ActionResult<Content>> GetContentByInviteCode([FromQuery] string inviteCode)
        {
            var content = await _patientService.GetContentByInviteCodeAsync(inviteCode);
            if (content != null)
            {
                return Ok(content);
            }
            return Unauthorized(new { message = "Invalid, expired, or fully used invite code." });
        }

        /// <summary>
        /// Records the patient's explicit consent before viewing content.
        /// Includes IP address for audit/compliance.
        /// </summary>
        [HttpPost("consent")]
        public async Task<IActionResult> RecordConsent([FromBody] PatientConsentDto consentDto)
        {
            // Retrieve the user's IP address from the current HTTP context
            var userIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";

            var success = await _patientService.RecordPatientConsentAsync(consentDto.InviteCode, userIpAddress);

            if (success)
            {
                return Ok(new { message = "Consent recorded successfully.", ip = userIpAddress });
            }
            return BadRequest(new { message = "Failed to record consent. Invalid invite code." });
        }

        /// <summary>
        /// Logs the patient's content engagement (view duration, city, language, etc.).
        /// </summary>
        [HttpPost("engagement")]
        public async Task<IActionResult> LogContentEngagement([FromBody] PatientEngagementDto engagementDto)
        {
            await _patientService.LogContentEngagementAsync(
                engagementDto.InviteCode,
                engagementDto.EngagementType, // e.g., "View", "Complete"
                engagementDto.DurationSeconds,
                engagementDto.City,
                engagementDto.Language
            );

            return Ok(new { message = "Engagement logged successfully." });
        }

        /// <summary>
        /// Logs the patient's completion and answers for an optional quiz.
        /// </summary>
        [HttpPost("quizzes/complete")]
        public async Task<IActionResult> LogQuizCompletion([FromBody] PatientQuizCompletionDto quizCompletionDto)
        {
            var success = await _patientService.LogQuizCompletionAsync(
                quizCompletionDto.InviteCode,
                quizCompletionDto.ContentId,
                quizCompletionDto.QuizResponses
            );

            if (success)
            {
                return Ok(new { message = "Quiz completion logged successfully." });
            }
            return BadRequest(new { message = "Failed to log quiz completion." });
        }
    }
}