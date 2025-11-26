using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using System;

namespace ClickHealthBackend.DTOs
{
    public class AnalyticsEventDto
    {
        public string PatientPseudoId { get; set; }
        public string InviteId { get; set; }
        public string HcpId { get; set; }
        public string ContentId { get; set; }
        public string AssetId { get; set; }
        public AssetType AssetType { get; set; }
        public string EventType { get; set; } // use strings to accept from front-end
        public double? ProgressPercent { get; set; }
        public int? PageNumber { get; set; }
        public string City { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
