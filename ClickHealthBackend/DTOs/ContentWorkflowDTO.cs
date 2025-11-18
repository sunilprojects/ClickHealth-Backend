namespace ClickHealthBackend.DTOs
{
    public class ContentWorkflowDTO
    {
        public string ContentId { get; set; }
        public string Content { get; set; }               // could be description or therapy
        public string ContentCreator { get; set; }        // UploadedByUserName
        public DateTime UploadedAt { get; set; }
        public string Status { get; set; }
        public string Approver { get; set; }             // ApprovedByUserName
        public string Notes { get; set; }                // ApproverNotes
    }
}
