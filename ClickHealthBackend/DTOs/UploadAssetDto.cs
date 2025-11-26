namespace ClickHealthBackend.DTOs
{
    public class UploadAssetDto
    {
        public string ContentId { get; set; }   // CT001
        public string Title { get; set; }
        public string Url { get; set; }
        public string Language { get; set; }    // en / hi
        public string AssetType { get; set; }   // PDF or VIDEO
        public int Version { get; set; } = 1;
    }
}
