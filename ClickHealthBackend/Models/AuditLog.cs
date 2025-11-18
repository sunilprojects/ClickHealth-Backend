using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ClickHealth.Server.Models
{
    public class AuditLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AuditLogId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonElement("entityType")]
        public string EntityType { get; set; } // e.g., "Content", "Campaign", "User", "Consent"

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("entityId")]
        public string EntityId { get; set; }

        [BsonElement("action")]
        public string Action { get; set; } // e.g., Create | Update | Delete | Approve | Access

        [BsonElement("oldValue")]
        public object OldValue { get; set; }

        [BsonElement("newValue")]
        public object NewValue { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [BsonElement("ipAddress")]
        public string IpAddress { get; set; }

        [BsonElement("userAgent")]
        public string UserAgent { get; set; }
    }
}
