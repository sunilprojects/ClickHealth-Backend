using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClickHealthBackend.Models
{
    public class PatientInvite
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string PatientId { get; set; }
        public string PatientCustomId { get; set; }
        public string CampaignId { get; set; }
        public string HcpId { get; set; }
        public string InviteId { get; set; }
        public DateTime InvitedAt { get; set; } = DateTime.UtcNow;

        public List<string> ViewedContentIds { get; set; } = new List<string>();
    }

}
