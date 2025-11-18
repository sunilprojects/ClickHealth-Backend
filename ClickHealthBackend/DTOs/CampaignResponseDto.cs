using ClickHealthBackend.Enums;
using System;
using System.Collections.Generic;

namespace ClickHealthBackend.Common.DTOs
{
    public class CampaignResponseDto
    {
        public string CampaignId { get; set; }
        public string Name { get; set; }
        public string Therapy { get; set; }
        public List<string> Cities { get; set; }
        public List<string> Territories { get; set; }
        public string Language { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // CRITICAL: Includes the status of the campaign
        public CampaignStatus Status { get; set; }

        public string CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Uses a simple dictionary for easy serialization to JSON, rather than the complex BsonDocument
        public Dictionary<string, object> TargetMetrics { get; set; }
    }
}