using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;

namespace ClickHealthBackend.DTOs
{
    public class AssetResponseDto
    {
        public string AssetId { get; set; }
        public string ContentId { get; set; }
        public AssetType AssetType { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Language { get; set; }
        public int Version { get; set; }
    }
}
