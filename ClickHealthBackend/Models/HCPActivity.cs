using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using ClickHealthBackend.Enums; 

namespace ClickHealthBackend.Models
{
    public class HCPActivity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string HcpActivityId { get; set; }

        [BsonElement("hcpCustomId")]
        public string HcpCustomId { get; set; }



        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("hcpUserId")]
        public string HcpUserId { get; set; }

        [BsonElement("hcpActivityType")]
        [BsonRepresentation(BsonType.String)]
        public HCPActivityType HcpActivityType { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("contentId")]
        public string ContentId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("campaignId")]
        public string CampaignId { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }

        [BsonElement("durationSeconds")]
        public int DurationSeconds { get; set; }

        [BsonElement("metadata")]
        public BsonDocument Metadata { get; set; }

        [BsonElement("territory")]
        public string Territory { get; set; }

        [BsonElement("city")]
        public string City { get; set; }
    }
}