using ClickHealthBackend.Enums;

namespace ClickHealthBackend.DTOs
{
    public class ContentDTO
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string FileType { get; set; }

        public string MedicalName { get; set; }
        public string ContentLanguage { get; set; }
        public string ContentDescription { get; set; }

        public string PdfUrl { get; set; }
        public string VideoUrl { get; set; }

        public DateTime? ReviewOn { get; set; }
        public DateTime? ExpiresOn { get; set; }

        public ContentStatus Status { get; set; }

        // Who uploaded?
        public string UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; }

        // Who approved?
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string ApproverNotes { get; set; }


    }
}

