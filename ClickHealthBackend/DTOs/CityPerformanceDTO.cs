using System;
using System.Collections.Generic;
namespace ClickHealthBackend.DTOs
{
    public class CityPerformanceDTO
    {
        public string City { get; set; }
        public int TotalCompletions { get; set; }
        public double EngagementPercentage { get; set; }
        public string LanguageMix { get; set; } // e.g., "Mixed", "English"
    }
}
