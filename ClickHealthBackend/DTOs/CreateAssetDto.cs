namespace ClickHealthBackend.DTOs
{
    public class CreateAssetDto
    {
        public string ContentId { get; set; }
        public string AssetType { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public string Version { get; set; }   // FIXED → must be string
    }

}
