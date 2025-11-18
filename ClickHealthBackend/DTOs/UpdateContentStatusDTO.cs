namespace ClickHealthBackend.DTOs
{
    public class UpdateContentStatusDTO
    {
        public string ContentId { get; set; }   // the content to update
        public string Status { get; set; }      // Approved / Rejected / PendingApproval
    }
}
