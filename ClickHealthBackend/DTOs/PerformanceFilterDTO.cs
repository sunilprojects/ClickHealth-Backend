
using System;
using System.Collections.Generic;
namespace ClickHealthBackend.DTOs
{
    public class PerformanceFilterDTO
    {
        public string City { get; set; } // Null for "All Cities"
        public string Language { get; set; } // Null for "All Languages"
        public DateTime? StartDate { get; set; } // Optional time range
        public DateTime? EndDate { get; set; } // Optional time range
    }
}
