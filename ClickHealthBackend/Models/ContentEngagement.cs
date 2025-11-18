using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

public class ContentEngagement
{
    // Corresponds to _id: string, PK
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string EngagemetId { get; set; }

    // Corresponds to contentId: string, FK (Refers to Content_id)
    [BsonElement("contentId")]
    public string ContentId { get; set; }

    // Corresponds to userId: string, FK (Refers to User_id (HCP or Patient))
    [BsonElement("userId")]
    public string UserId { get; set; }

    // Corresponds to userType: string (HCP | Patient)
    [BsonElement("userType")]
    public string UserType { get; set; }

    // Corresponds to campaignId: string, FK (Refers to Campaign_id)
    [BsonElement("campaignId")]
    public string CampaignId { get; set; }

    // Corresponds to timestamp: datetime
    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; }

    // Corresponds to engagementType: string (View | Share | Complete | Download)
    [BsonElement("engagementType")]
    public string EngagementType { get; set; }

    // Corresponds to durationSeconds: int
    [BsonElement("durationSeconds")]
    public int DurationSeconds { get; set; }

    // Corresponds to city: string
    [BsonElement("city")]
    public string City { get; set; }

    // Corresponds to territory: string
    [BsonElement("territory")]
    public string Territory { get; set; }

    // Corresponds to language: string
    [BsonElement("language")]
    public string Language { get; set; }
}