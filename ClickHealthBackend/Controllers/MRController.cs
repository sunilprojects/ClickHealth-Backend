using Microsoft.AspNetCore.Mvc;
using ClickHealthBackend.Services.Interfaces;
using ClickHealthBackend.DTOs;
using System.Threading.Tasks;
namespace ClickHealthBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MRController : ControllerBase
    {
        private readonly IMRService _mrService;

        public MRController(IMRService mrService)
        {
            _mrService = mrService;
        }

        /// <summary>
        /// Registers a new HCP account, sets it to active, and triggers a welcome email with the password.
        /// </summary>
        [HttpPost("onboard/hcp")]
        public async Task<IActionResult> OnboardHcp([FromBody] HcpOnboardDto onboardDto)
        {
            // NOTE: onboardDto.MrUserId should be retrieved from the authenticated user's session/claim
            var hcpUserId = await _mrService.OnboardHCPAsync(
                onboardDto.MrUserId,
                onboardDto.HcpName,
                onboardDto.HcpEmail,
                onboardDto.HcpPhone,
                onboardDto.Territory,
                onboardDto.Specialty
            );
            if (!string.IsNullOrEmpty(hcpUserId))
            {
                return Created("HCP Onboarded for immediate access", new { hcpUserId });
            }
            return BadRequest(new { message = "Failed to onboard HCP." });
        }

        /// <summary>
        /// Logs the assignment of an approved Campaign from the MR to a specific HCP.
        /// </summary>
        [HttpPost("share-campaign")]
        public async Task<IActionResult> ShareCampaign([FromBody] ContentShareDto shareDto)
        {
            // NOTE: shareDto.MrUserId should be retrieved from the authenticated user's session/claim
            // The service checks if the campaign is active before logging the share.
            var success = await _mrService.ShareContentWithHCPAsync(shareDto.MrUserId, shareDto.HcpUserId, shareDto.CampaignId);
            if (success)
            {
                return Ok(new { message = "Campaign assigned successfully and activity logged." });
            }
            return BadRequest(new { message = "Failed to share campaign. It may not be active." });
        }

        /// <summary>
        /// Logs qualitative feedback from the field (e.g., meeting notes).
        /// </summary>
        [HttpPost("feedback")]
        public async Task<IActionResult> LogFieldFeedback([FromBody] FieldFeedbackDto feedbackDto)
        {
            // NOTE: feedbackDto.MrUserId should be retrieved from the authenticated user's session/claim
            await _mrService.LogFieldFeedbackAsync(feedbackDto.MrUserId, feedbackDto.Notes);
            return Ok(new { message = "Feedback logged successfully." });
        }

        /// <summary>
        /// Retrieves the real-time snapshot of active HCPs and patient engagement in the MR's territory.
        /// </summary>
        [HttpGet("dashboard/snapshot")]
        public async Task<IActionResult> GetTerritorySnapshot([FromQuery] string mrUserId)
        {
            // NOTE: mrUserId should be retrieved from the authenticated user's session/claim
            var snapshot = await _mrService.GetTerritorySnapshotAsync(mrUserId);
            if (snapshot != null)
            {
                return Ok(snapshot);
            }
            return NotFound(new { message = "MR not found or territory data unavailable." });
        }
    }
}