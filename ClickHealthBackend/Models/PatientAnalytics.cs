using ClickHealthBackend.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ClickHealthBackend.Models
{
    public class PatientAnalytics
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string PatientPseudoId { get; set; }  // pseudonymous id

        public string InviteId { get; set; }         // INVxxx

        public string HcpId { get; set; }

        public string ContentId { get; set; }        // CT001

        public string AssetId { get; set; }          // AS001

        public AssetType AssetType { get; set; }

        public AnalyticsEventType EventType { get; set; }

        public double? ProgressPercent { get; set; } // for video

        public int? PageNumber { get; set; }         // for pdf

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string City { get; set; }             // optional for heatmaps
    }
}
