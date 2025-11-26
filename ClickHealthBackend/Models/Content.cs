using ClickHealthBackend.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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
        public string Therapy { get; set; }

        [BsonElement("language")]
        public string Language { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("upload_pdf")]
        public string FileUrl { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("contentType")]
        public ContentType ContentType { get; set; }

        [BsonElement("video_url")]
        public string ThumbnailUrl { get; set; }

        public string ContentUrl { get; set; }   // <-- REQUIRED FIELD

        public string FileType { get; set; }     // video/pdf/image/etc.

        [BsonElement("reviewDate")]
        public DateTime? ReviewDate { get; set; }

        [BsonElement("expiryDate")]
        public DateTime? ExpiryDate { get; set; }

        [BsonElement("uploadedAt")]
        public DateTime UploadedAt { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public ContentStatus Status { get; set; }

        [BsonElement("uploadedBy_user_id")]
        public string UploadedByUserId { get; set; }

        [BsonElement("UploadedByUserName")]
        public string UploadedByUserName { get; set; }

        // FIXED: Nested metadata object
        [BsonElement("metadata")]
        public ContentMetadata Metadata { get; set; }

        [BsonElement("approvedAt")]
        public DateTime? ApprovedAt { get; set; }

        [BsonElement("pdfCustomId")]
        public string PdfCustomId { get; set; }

        [BsonElement("videoCustomId")]
        public string VideoCustomId { get; set; }
    }

    public class ContentMetadata
    {
        [BsonElement("ApprovedByUserId")]
        public string ApprovedByUserId { get; set; }

        [BsonElement("ApprovedByUserName")]
        public string ApprovedByUserName { get; set; }

        [BsonElement("ApproverNotes")]
        public string ApproverNotes { get; set; }
    }
}
