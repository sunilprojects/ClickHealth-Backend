using ClickHealthBackend.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ClickHealthBackend.Models
{
    [BsonIgnoreExtraElements]
    public class Content
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonElement("_id")]
        public string ContentId { get; set; }


        [BsonElement("contentCustomId")]
        public string ContentCustomId { get; set; }


        [BsonElement("medical_name")]
        public string Therapy { get; set; }           // corresponds to "medical_name"

        [BsonElement("language")]
        public string Language { get; set; }          // corresponds to "language"

        [BsonElement("description")]
        public string Description { get; set; }       // corresponds to "description"

        [BsonElement("upload_pdf")]
        public string FileUrl { get; set; }           // corresponds to "upload_pdf"

        [BsonRepresentation(BsonType.String)]
        [BsonElement("contentType")]
        public ContentType ContentType { get; set; }  // corresponds to "contentType"

        [BsonElement("video_url")]
        public string ThumbnailUrl { get; set; }      // corresponds to "video_url"

        [BsonElement("reviewDate")]
        public DateTime? ReviewDate { get; set; }     // corresponds to "reviewDate"

        [BsonElement("expiryDate")]
        public DateTime? ExpiryDate { get; set; }     // corresponds to "expiryDate"

        [BsonElement("uploadedAt")]
        public DateTime UploadedAt { get; set; }      // corresponds to "uploadedAt"

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public ContentStatus Status { get; set; }     // corresponds to "status"

        [BsonElement("uploadedBy_user_id")]
        public string UploadedByUserId { get; set; }  // corresponds to "uploadedBy_user_id"

        [BsonElement("UploadedByUserName")]
        public string UploadedByUserName { get; set; } // corresponds to "UploadedByUserName"

        // Nested metadata fields
        [BsonElement("metadata.ApprovedByUserId")]
        public string ApprovedByUserId { get; set; }

        [BsonElement("metadata.ApprovedByUserName")]
        public string ApprovedByUserName { get; set; }

        [BsonElement("metadata.ApproverNotes")]
        public string ApproverNotes { get; set; }
       
        [BsonElement("approvedAt")]
        public DateTime? ApprovedAt { get; set; }


    }
}