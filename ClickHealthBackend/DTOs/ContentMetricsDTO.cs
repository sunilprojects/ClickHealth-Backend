
using System;
using System.Collections.Generic;
namespace ClickHealthBackend.DTOs
{
    public class ContentMetricsDTO
    {
        public string ContentId { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; } // e.g., "Video", "PDF"
        public string Language { get; set; }
        public int Completions { get; set; }
        public double AverageDwellTimeMinutes { get; set; }
        public double ShareRatePercentage { get; set; }
        public string PerformanceRating { get; set; } // High, Medium, Low
        public string ActionRecommendation { get; set; } // Promote, Replace, Optimize
    }
}
