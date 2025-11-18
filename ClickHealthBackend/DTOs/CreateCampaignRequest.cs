
namespace ClickHealthBackend.DTOs
{

    public class CreateCampaignRequest
    {
        public string Name { get; set; }
        public string Therapy { get; set; }
        public string Language { get; set; }
        public List<string> Cities { get; set; }
        public List<string> Territories { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CreatedByUserId { get; set; }
        public List<string> ContentIds { get; set; }
    }

}