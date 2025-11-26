using ClickHealthBackend.DTOs;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace ClickHealthBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientRepository _patientRepo;
        private readonly IPatientInviteRepository _inviteRepo;
        private readonly IEmailService _emailService;
        private readonly ICampaignRepository _campaignRepo;

        public PatientsController(
            IPatientRepository patientRepo,
            IPatientInviteRepository inviteRepo,
            IEmailService emailService,
            ICampaignRepository campaignRepo)
        {
            _patientRepo = patientRepo;
            _inviteRepo = inviteRepo;
            _emailService = emailService;
            _campaignRepo = campaignRepo;
        }

        // ==========================================================
        // 1) UPLOAD EXCEL → FILTER SPECIALTY → ONLY SEND MATCHING
        // ==========================================================
        [HttpPost("upload-excel/{campaignId}/{hcpId}")]
        public async Task<IActionResult> UploadPatients(IFormFile file, string campaignId, string hcpId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please upload an Excel file.");

            var campaign = await _campaignRepo.GetByIdAsync(campaignId);
            if (campaign == null)
                return BadRequest("Invalid campaign.");

            string requiredSpecialty = campaign.Specialty?.Trim() ?? "";
            var excelPatients = new List<PatientExcelRow>();
            var errors = new List<string>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    var sheet = package.Workbook.Worksheets[0];
                    int rows = sheet.Dimension.End.Row;

                    for (int row = 2; row <= rows; row++)
                    {
                        string name = sheet.Cells[row, 1].Text?.Trim();
                        string email = sheet.Cells[row, 2].Text?.Trim();
                        string specialty = sheet.Cells[row, 6].Text?.Trim();

                        bool isEmpty =
                            string.IsNullOrWhiteSpace(name) &&
                            string.IsNullOrWhiteSpace(email) &&
                            string.IsNullOrWhiteSpace(specialty);

                        if (isEmpty)
                            continue;

                        var p = new PatientExcelRow
                        {
                            Name = name,
                            Email = email,
                            Phone = sheet.Cells[row, 3].Text?.Trim(),
                            Age = sheet.Cells[row, 4].Text?.Trim(),
                            Gender = sheet.Cells[row, 5].Text?.Trim(),
                            Specialty = specialty,
                            Condition = sheet.Cells[row, 7].Text?.Trim(),
                            Language = sheet.Cells[row, 8].Text?.Trim()
                        };

                        // Name + email basic validation
                        if (string.IsNullOrWhiteSpace(p.Name))
                            errors.Add($"Row {row}: Name is required.");

                        if (string.IsNullOrWhiteSpace(p.Email))
                            errors.Add($"Row {row}: Email is required.");
                        else if (!p.Email.Contains("@"))
                            errors.Add($"Row {row}: '{p.Email}' is not a valid email.");

                        excelPatients.Add(p);
                    }
                }
            }

            // If error exist → return only validation errors (NOT specialty mismatch)
            if (errors.Any())
            {
                return BadRequest(new
                {
                    message = "Excel validation failed.",
                    errors
                });
            }

            // ==========================================================
            // 2) PROCESS ONLY PATIENTS WITH MATCHING SPECIALTY
            // ==========================================================
            var matchedPatients = excelPatients
                .Where(p => string.Equals(p.Specialty, requiredSpecialty, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!matchedPatients.Any())
                return Ok("No patients matched the campaign specialty. No invites sent.");

            // ==========================================================
            // 3) SAVE / REUSE PATIENT → SEND INVITE EMAILS
            // ==========================================================
            foreach (var p in matchedPatients)
            {
                Patient patient;

                var existing = await _patientRepo.GetByEmailAsync(p.Email);

                if (existing == null)
                {
                    string customId = await _patientRepo.GeneratePatientCustomIdAsync();

                    patient = new Patient
                    {
                        PatientCustomId = customId,
                        Name = p.Name,
                        Email = p.Email,
                        Phone = p.Phone,
                        Age = p.Age,
                        Gender = p.Gender,
                        Specialty = p.Specialty,
                        Condition = p.Condition,
                        Language = p.Language,
                        HcpId = hcpId
                    };

                    await _patientRepo.CreateAsync(patient);
                }
                else
                {
                    patient = existing;
                }

                // Create invite entry
                var invite = new PatientInvite
                {
                    PatientId = patient.Id,
                    PatientCustomId = patient.PatientCustomId,
                    CampaignId = campaignId,
                    HcpId = hcpId,
                    InviteId = Guid.NewGuid().ToString(),
                    InvitedAt = DateTime.UtcNow
                };

                await _inviteRepo.CreateAsync(invite);

                // EMAIL LINK
                string link =
                    $"https://localhost:7286/api/patients/dashboard?pid={patient.PatientCustomId}&cid={campaignId}";

                await _emailService.SendEmailAsync(
                    patient.Email,
                    "Your Health Content",
                    $"Hello {patient.Name},<br/><br/>Please click the link below to view your health content:<br/><a href='{link}'>{link}</a>"
                );
            }

            return Ok("Invites sent to matching-specialty patients successfully.");
        }

        // ==========================================================
        // 4) PATIENT DASHBOARD
        // ==========================================================
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard(string pid, string cid, [FromServices] IContentRepository contentRepo)
        {
            var invite = await _inviteRepo.GetByPatientAndCampaignAsync(pid, cid);

            if (invite == null)
                return Ok(new PatientDashboardDTO { IsValid = false, Message = "Invalid or expired link." });

            var patient = await _patientRepo.GetByCustomIdAsync(pid);
            if (patient == null)
                return Ok(new PatientDashboardDTO { IsValid = false, Message = "Patient not found." });

            var campaign = await _campaignRepo.GetByIdAsync(cid);
            if (campaign == null)
                return Ok(new PatientDashboardDTO { IsValid = false, Message = "Campaign not found." });

            var contents = await contentRepo.GetByIdsAsync(campaign.ContentIds);

            return Ok(new PatientDashboardDTO
            {
                IsValid = true,
                Message = "Success",

                PatientName = patient.Name,
                PatientCustomId = patient.PatientCustomId,
                CampaignId = campaign.CampaignId,
                HcpId = invite.HcpId,

                ViewedContentIds = invite.ViewedContentIds ?? new List<string>(),

                Contents = contents.Select(x => new ContentDTO
                {
                    MedicalName = x.Therapy,
                    ContentLanguage = x.Language,
                    ContentDescription = x.Description,
                    PdfUrl = x.FileUrl,
                    VideoUrl = x.ThumbnailUrl,
                    ReviewOn = x.ReviewDate,
                    ExpiresOn = x.ExpiryDate,
                    Status = x.Status
                }).ToList()
            });
        }
    }
}
