using Microsoft.AspNetCore.Http;
namespace ClickHealthBackend.DTOs
{
    public class ContentUploadRequest
    {
        public string MedicalName { get; set; } = "";
        public string ContentLanguage { get; set; } = "";
        public string ContentDescription { get; set; } = "";
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public IFormFile? Pdf { get; set; }
        public IFormFile? Video { get; set; }
    }

}
