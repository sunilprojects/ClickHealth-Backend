
using ClickHealthBackend.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ClickHealthBackend.Models
{
    public class Campaign
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("campaign_id")]
        public string CampaignId { get; set; }

        [BsonElement("campaignCustomId")]
        public string CampaignCustomId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("therapy")]
        public string Therapy { get; set; }

        [BsonElement("cities")]
        public List<string> Cities { get; set; } = new();

        [BsonElement("territories")]
        public List<string> Territories { get; set; } = new();

        [BsonElement("language")]
        public string Language { get; set; } // EN | Hindi

        [BsonElement("startDate")]
        public DateTime StartDate { get; set; }

        [BsonElement("endDate")]
        public DateTime EndDate { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("status")]
        public CampaignStatus Status { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("createdBy_user_id")]
        public string CreatedByUserId { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("targetMetrics")]
        public BsonDocument TargetMetrics { get; set; }

        [BsonElement("contentIds")]
        public List<string> ContentIds { get; set; } = new();

    }
}
