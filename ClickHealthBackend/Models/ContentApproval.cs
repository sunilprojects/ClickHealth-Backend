using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

public class ContentApproval
{
    // Corresponds to _id: string, PK
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ContApprovalId { get; set; }

    // Corresponds to contentId: string, PK (Part of a composite key if not using a separate _id)
    // Since _id is PK, contentId is likely an index or part of a unique constraint.
    // Assuming the table structure implies the approval record IS the content record.
    // However, based on the schema, it's a separate entity.
    [BsonElement("contentId")]
    public string ContentId { get; set; }

    // Corresponds to approverUserId: string, FK (Refers to User_id)
    [BsonElement("approverUserId")]
    public string ApproverUserId { get; set; }

    // Corresponds to status: string (Pending | Approved | Rejected)
    [BsonElement("status")]
    public string Status { get; set; }

    // Corresponds to version: string
    [BsonElement("version")]
    public string Version { get; set; }

    // Corresponds to comments: string
    [BsonElement("comments")]
    public string Comments { get; set; }

    // Corresponds to approvedAt: datetime
    [BsonElement("approvedAt")]
    public DateTime? ApprovedAt { get; set; } // Use nullable DateTime

    // Corresponds to expiresAt: datetime
    [BsonElement("expiresAt")]
    public DateTime? ExpiresAt { get; set; } // Use nullable DateTime
}