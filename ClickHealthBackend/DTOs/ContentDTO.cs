using ClickHealthBackend.Enums;

namespace ClickHealthBackend.DTOs
{
    public class ContentDTO
    {
        public string MedicalName { get; set; }       // was Therapy
        public string ContentLanguage { get; set; }   // was Language
        public string ContentDescription { get; set; } // was Description
        public string PdfUrl { get; set; }            // was FileUrl
        public string VideoUrl { get; set; }          // was ThumbnailUrl
                                                     
        public DateTime? ReviewOn { get; set; } // ✅ Nullable — optional scheduling
        public DateTime? ExpiresOn { get; set; }     // was ExpiryDate


        public ContentStatus Status { get; set; }
    }
}