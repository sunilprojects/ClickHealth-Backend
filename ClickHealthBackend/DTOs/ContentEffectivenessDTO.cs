
using System;
using System.Collections.Generic;
namespace ClickHealthBackend.DTOs
{
    public class ContentEffectivenessDTO
    {
        public List<ContentMetricsDTO> TopPerformingAssets { get; set; }
        public List<ContentMetricsDTO> UnderperformingContent { get; set; }
    }
}
