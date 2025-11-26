using ClickHealthBackend.DTOs;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClickHealthBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly IContentService _contentService;

        public ContentController(IContentService contentService)
        {
            _contentService = contentService;
        }

        // --- Upload new content ---
        [HttpPost("upload")]
        [RequestSizeLimit(600_000_000)] // 600 MB
        public async Task<IActionResult> UploadContent(
            [FromForm] ContentUploadRequest request,
            [FromForm] string uploaderCustomId,
            [FromForm] string uploaderName)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.MedicalName))
                return BadRequest("Invalid request: MedicalName is required.");

            if (string.IsNullOrWhiteSpace(uploaderCustomId))
                return BadRequest("UploaderCustomId is required.");

            if (string.IsNullOrWhiteSpace(uploaderName))
                return BadRequest("UploaderName is required.");

            string pdfUrl = string.Empty;
            string videoUrl = string.Empty;

            // --- Save PDF ---
            if (request.Pdf != null)
            {
                var pdfFileName = $"{Guid.NewGuid()}_{Path.GetFileName(request.Pdf.FileName)}";
                var pdfDir = Path.Combine("wwwroot", "uploads", "pdf");
                Directory.CreateDirectory(pdfDir);

                var pdfPath = Path.Combine(pdfDir, pdfFileName);
                await using var pdfStream = new FileStream(pdfPath, FileMode.Create);
                await request.Pdf.CopyToAsync(pdfStream);

                pdfUrl = $"{Request.Scheme}://{Request.Host}/uploads/pdf/{pdfFileName}";
            }

            // --- Save Video ---
            if (request.Video != null)
            {
                var videoFileName = $"{Guid.NewGuid()}_{Path.GetFileName(request.Video.FileName)}";
                var videoDir = Path.Combine("wwwroot", "uploads", "video");
                Directory.CreateDirectory(videoDir);

                var videoPath = Path.Combine(videoDir, videoFileName);
                await using var videoStream = new FileStream(videoPath, FileMode.Create);
                await request.Video.CopyToAsync(videoStream);

                videoUrl = $"{Request.Scheme}://{Request.Host}/uploads/video/{videoFileName}";
            }

            // --- Convert to DTO ---
            var contentDto = new ContentDTO
            {
                MedicalName = request.MedicalName,
                ContentLanguage = request.ContentLanguage,
                ContentDescription = request.ContentDescription,
                PdfUrl = pdfUrl,
                VideoUrl = videoUrl,
                ExpiresOn = request.ExpiresOn ?? DateTime.UtcNow.AddMonths(1),
                ReviewOn = request.StartDate ?? DateTime.UtcNow,
                Status = ContentStatus.Pending
            };

            var contentId = await _contentService.UploadContentAsync(
                contentDto,
                uploaderCustomId,
                uploaderName
            );

            return Ok(new
            {
                ContentId = contentId,
                PdfUrl = pdfUrl,
                VideoUrl = videoUrl,
                Message = "✅ Content uploaded successfully."
            });
        }

        // --- Get content by status ---
        [HttpGet("by-status")]
        [ProducesResponseType(typeof(List<MedicalContentResponseDto>), 200)]
        public async Task<IActionResult> GetContentsByStatus([FromQuery] ContentStatus status)
        {
            var contents = await _contentService.GetContentsByStatusAsync(status);
            return Ok(contents ?? new List<MedicalContentResponseDto>());
        }

        // --- Workflow by uploader ---
        [HttpGet("workflow")]
        [ProducesResponseType(typeof(List<ContentWorkflowDTO>), 200)]
        public async Task<IActionResult> GetWorkflow([FromQuery] string uploaderId)
        {
            if (string.IsNullOrWhiteSpace(uploaderId))
                return BadRequest("UploaderId is required.");

            var workflow = await _contentService.GetContentWorkflowByUploaderAsync(uploaderId);
            return Ok(workflow ?? new List<ContentWorkflowDTO>());
        }

        // --- Update content status ---
        [HttpPut("update-status/{contentCustomId}")]
        public async Task<IActionResult> UpdateContentStatus(
            string contentCustomId,
            [FromQuery] ContentStatus newStatus,
            [FromQuery] string approverCustomId,
            [FromQuery] string approverName,
            [FromQuery] string? remarks)
        {
            if (string.IsNullOrWhiteSpace(contentCustomId))
                return BadRequest("ContentCustomId is required.");

            if (string.IsNullOrWhiteSpace(approverCustomId))
                return BadRequest("ApproverCustomId is required.");

            if (string.IsNullOrWhiteSpace(approverName))
                return BadRequest("ApproverName is required.");

            var updated = await _contentService.UpdateContentStatusByCustomIdAsync(
                contentCustomId,
                newStatus,
                approverCustomId,
                approverName,
                remarks
            );

            if (!updated)
                return NotFound(new { Message = $"❌ No content found with Custom ID: {contentCustomId}" });

            return Ok(new
            {
                ContentCustomId = contentCustomId,
                Status = newStatus.ToString(),
                ApprovedBy = approverName,
                ApproverCustomId = approverCustomId,
                ApprovedOn = DateTime.UtcNow,
                Remarks = remarks,
                Message = "✅ Content status updated successfully."
            });
        }

        // --- Get approved/rejected/pending ---
        [HttpGet("approved")]
        public async Task<IActionResult> GetApprovedContents() =>
            Ok(await _contentService.GetContentsByStatusAsync(ContentStatus.Approved) ?? new List<MedicalContentResponseDto>());

        [HttpGet("rejected")]
        public async Task<IActionResult> GetRejectedContents() =>
            Ok(await _contentService.GetContentsByStatusAsync(ContentStatus.Rejected) ?? new List<MedicalContentResponseDto>());

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingContents() =>
            Ok(await _contentService.GetContentsByStatusAsync(ContentStatus.Pending) ?? new List<MedicalContentResponseDto>());
    }
}
