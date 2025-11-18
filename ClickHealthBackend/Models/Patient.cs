
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClickHealthBackend.Models
{
    public class Patient
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("inviteCode")]
        public string InviteCode { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("hcpUserId")]
        public string HcpUserId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("contentId")]
        public string ContentId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("campaignId")]
        public string CampaignId { get; set; }

        [BsonElement("expiresAt")]
        public DateTime ExpiresAt { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; }

        [BsonElement("maxUses")]
        public int MaxUses { get; set; }

        [BsonElement("usedCount")]
        public int UsedCount { get; set; }

        [BsonElement("shareChannel")]
        public string ShareChannel { get; set; }
    }
}