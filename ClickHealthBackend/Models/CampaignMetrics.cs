
using MongoDB.Bson.Serialization.Attributes;

namespace ClickHealthBackend.Models
{
    public class CampaignMetrics
    {
        [BsonElement("campaignId")]
        public string CampaignId { get; set; } = string.Empty;

        [BsonElement("metricsDate")]
        public DateTime MetricsDate { get; set; }

        [BsonElement("city")]
        public string? City { get; set; }

        [BsonElement("territory")]
        public string? Territory { get; set; }

        [BsonElement("hcpActiveCount")]
        public int HcpActiveCount { get; set; }

        [BsonElement("hcpShareCount")]
        public int HcpShareCount { get; set; }

        [BsonElement("patientInviteCount")]
        public int PatientInviteCount { get; set; }

        [BsonElement("patientCompletionCount")]
        public int PatientCompletionCount { get; set; }

        [BsonElement("contentPerformance")]
        public Dictionary<string, ContentPerformance>? ContentPerformance { get; set; }

        [BsonElement("calculatedAt")]
        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ContentPerformance
    {
        [BsonElement("views")]
        public int Views { get; set; }

        [BsonElement("completions")]
        public int Completions { get; set; }

        [BsonElement("shares")]
        public int Shares { get; set; }

        [BsonElement("avgDurationSeconds")]
        public int AvgDurationSeconds { get; set; }
    }
}
