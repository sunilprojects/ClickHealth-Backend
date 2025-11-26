namespace ClickHealthBackend.DTOs
{
    public class PatientDashboardDTO
    {
        public string PatientName { get; set; }
        public string PatientCustomId { get; set; }

        public string CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string Therapy { get; set; }
        public List<string> ContentUrls { get; set; }
        public string Language { get; set; }

        public bool IsValid { get; set; }
        public string Message { get; set; }

        public string HcpId { get; set; }

        // If system tracks viewed content
        public List<string> ViewedContentIds { get; set; }

        public List<ContentDTO> Contents { get; set; }


        // Full content list (if needed)
        public List<ContentDTO> ContentList { get; set; }
    }
}
