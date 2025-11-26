using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClickHealthBackend.Models
{
    public class ContentAnalytics
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string AnalyticsId { get; set; } // AN001

        public string AssetId { get; set; }     // AS001
        public string ContentId { get; set; }   // CT001

        public string PatientId { get; set; }
        public string HcpId { get; set; }

        public bool IsPdfOpened { get; set; }
        public int VideoWatchPercentage { get; set; } // 0-100 %

        public string Feedback { get; set; }

        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    }
}
