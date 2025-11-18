using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using ClickHealthBackend.Enums; 

namespace ClickHealthBackend.Models
{
    public class MRActivity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("mrUserId")]
        public string MrUserId { get; set; }

        [BsonElement("mrActivityType")]
        [BsonRepresentation(BsonType.String)]
        public MRActivityType MrActivityType { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("hcpUserId")]
        public string HcpUserId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("campaignId")]
        public string CampaignId { get; set; }

        [BsonElement("territory")]
        public string Territory { get; set; }

        [BsonElement("city")]
        public string City { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }

        [BsonElement("feedback")]
        public string Feedback { get; set; }

        [BsonElement("notes")]
        public string Notes { get; set; }
    }
}