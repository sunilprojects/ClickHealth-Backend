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

        // ✅ Upload new content (PDF + Video)
        [HttpPost("upload")]
        [RequestSizeLimit(600_000_000)] // 600 MB
        public async Task<IActionResult> UploadContent([FromForm] ContentUploadRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.MedicalName))
                return BadRequest("Invalid request: MedicalName is required.");

            string pdfUrl = string.Empty;
            string videoUrl = string.Empty;

            // --- Save PDF ---
            if (request.Pdf != null)
            {
                var pdfFileName = $"{Guid.NewGuid()}_{Path.GetFileName(request.Pdf.FileName)}";
                var pdfDir = Path.Combine("wwwroot", "uploads", "pdf");
                Directory.CreateDirectory(pdfDir);

                var pdfPath = Path.Combine(pdfDir, pdfFileName);
                await using var stream = new FileStream(pdfPath, FileMode.Create);
                await request.Pdf.CopyToAsync(stream);

                pdfUrl = $"{Request.Scheme}://{Request.Host}/uploads/pdf/{pdfFileName}";
            }

            // --- Save Video ---
            if (request.Video != null)
            {
                var videoFileName = $"{Guid.NewGuid()}_{Path.GetFileName(request.Video.FileName)}";
                var videoDir = Path.Combine("wwwroot", "uploads", "video");
                Directory.CreateDirectory(videoDir);

                var videoPath = Path.Combine(videoDir, videoFileName);
                await using var stream = new FileStream(videoPath, FileMode.Create);
                await request.Video.CopyToAsync(stream);

                videoUrl = $"{Request.Scheme}://{Request.Host}/uploads/video/{videoFileName}";
            }

            // --- DTO for DB ---
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

            string uploaderId = "TEMP123";
            string uploaderName = "TeamUser";

            var contentId = await _contentService.UploadContentAsync(contentDto, uploaderId, uploaderName);

            return Ok(new
            {
                ContentId = contentId,
                PdfUrl = pdfUrl,
                VideoUrl = videoUrl,
                Message = "✅ Content uploaded successfully."
            });
        }

        // ✅ Get Approved
        [HttpGet("approved")]
        public async Task<IActionResult> GetApprovedContents()
        {
            var contents = await _contentService.GetContentsByStatusAsync(ContentStatus.Approved);
            return Ok(contents ?? new List<MedicalContentResponseDto>());
        }

        // ✅ Get Rejected
        [HttpGet("rejected")]
        public async Task<IActionResult> GetRejectedContents()
        {
            var contents = await _contentService.GetContentsByStatusAsync(ContentStatus.Rejected);
            return Ok(contents ?? new List<MedicalContentResponseDto>());
        }

        // ✅ Get Pending
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingContents()
        {
            var contents = await _contentService.GetContentsByStatusAsync(ContentStatus.Pending);
            return Ok(contents ?? new List<MedicalContentResponseDto>());
        }

        // ✅ Workflow
        [HttpGet("workflow")]
        public async Task<IActionResult> GetWorkflow([FromQuery] string uploaderId)
        {
            if (string.IsNullOrWhiteSpace(uploaderId))
                return BadRequest("UploaderId is required.");

            var workflow = await _contentService.GetContentWorkflowByUploaderAsync(uploaderId);
            return Ok(workflow ?? new List<ContentWorkflowDTO>());
        }

        // ✅ Update status
        [HttpPut("update-status/{contentId}")]
        public async Task<IActionResult> UpdateContentStatus(
            string contentId,
            [FromQuery] ContentStatus newStatus,
            [FromQuery] string? notes)
        {
            if (string.IsNullOrWhiteSpace(contentId))
                return BadRequest("Content ID is required.");

            string approverName = "ProductManager01";

            var updated = await _contentService.UpdateContentStatusAsync(contentId, newStatus, approverName, notes);

            if (!updated)
                return NotFound(new { Message = $"❌ No content found with ID: {contentId}" });

            return Ok(new
            {
                ContentId = contentId,
                Status = newStatus.ToString(),
                Message = $"✅ Content status updated to '{newStatus}' by {approverName}."
            });
        }
    }
}
