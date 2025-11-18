
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClickHealthBackend.Models
{
    public class PatientEngagement
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("inviteCode")]
        public string InviteCode { get; set; }

        [BsonElement("pseudonymousId")]
        public string PseudonymousId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("contentId")]
        public string ContentId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("campaignId")]
        public string CampaignId { get; set; }

        [BsonElement("viewedAt")]
        public DateTime? ViewedAt { get; set; }

        [BsonElement("completedAt")]
        public DateTime? CompletedAt { get; set; }

        [BsonElement("durationSeconds")]
        public int DurationSeconds { get; set; }

        [BsonElement("consentGiven")]
        public bool ConsentGiven { get; set; }

        [BsonElement("quizResponse")]
        public BsonDocument QuizResponse { get; set; }

        [BsonElement("city")]
        public string City { get; set; }

        [BsonElement("language")]
        public string Language { get; set; }

        [BsonElement("engagementType")]
        [BsonRepresentation(BsonType.String)]
        public EngagementType? EngagementType { get; set; }
    }
}