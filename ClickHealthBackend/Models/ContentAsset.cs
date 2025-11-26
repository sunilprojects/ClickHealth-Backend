using ClickHealthBackend.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClickHealthBackend.Models
{
    public class ContentAsset
    {
        [BsonId]
        public string AssetId { get; set; }

        public string ContentId { get; set; }

        public AssetType AssetType { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public string Language { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }


}
