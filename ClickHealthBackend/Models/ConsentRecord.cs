using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ClickHealth.Server.Models
{
    public class ConsentRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ContRecordId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("userType")]
        public string UserType { get; set; } // e.g., HCP, Patient

        [BsonElement("consentType")]
        public string ConsentType { get; set; } // e.g., DPDP, Marketing, DataProcessing

        [BsonElement("isGranted")]
        public bool IsGranted { get; set; }

        [BsonElement("grantedAt")]
        public DateTime? GrantedAt { get; set; }

        [BsonElement("revokedAt")]
        public DateTime? RevokedAt { get; set; }

        [BsonElement("version")]
        public string Version { get; set; }

        [BsonElement("ipAddress")]
        public string IpAddress { get; set; }
        public string UserIpAddress { get; internal set; }
    }
}
