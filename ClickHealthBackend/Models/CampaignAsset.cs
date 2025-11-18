using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class CampaignAsset
{
    // Corresponds to _id: string, PK
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)] // Assuming standard MongoDB ObjectId for _id
    public string CampAssetId { get; set; }

    // Corresponds to _campaignId: string, FK (Refers to Campaign_id)
    // Could also be stored as ObjectId if Campaign_id is an ObjectId
    [BsonElement("_campaignId")]
    public string CampaignId { get; set; }

    // Corresponds to contentId: string, FK (Refers to Content_id)
    [BsonElement("contentId")]
    public string ContentId { get; set; }

    // Corresponds to displayOrder: int
    [BsonElement("displayOrder")]
    public int DisplayOrder { get; set; }

    // Corresponds to isActive: bool
    [BsonElement("isActive")]
    public bool IsActive { get; set; }
}