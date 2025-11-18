using ClickHealthBackend.DTOs;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Interfaces;
using System.Collections.Generic;
using System.Linq; // Added for .Any() and .Average()
using System.Threading.Tasks;

namespace ClickHealthBackend.Services.Implementations
{
    public class ContentMetricsService : IContentMetricsService
    {
        private readonly IContentRepository _contentRepo;

        public ContentMetricsService(IContentRepository contentRepo)
        {
            _contentRepo = contentRepo;
        }

        public async Task<List<CityPerformanceDTO>> GetCityPerformanceWithFiltersAsync(PerformanceFilterDTO filter)
        {

            return await _contentRepo.GetCityPerformanceAsync(filter);
        }

        public async Task<ContentEffectivenessDTO> GetContentEffectivenessAsync(PerformanceFilterDTO filter)
        {
            // 1. Get raw metrics from repository
            var allMetrics = await _contentRepo.GetContentMetricsAsync(filter);

            if (!allMetrics.Any())
            {
                return new ContentEffectivenessDTO
                {
                    TopPerformingAssets = new(),
                    UnderperformingContent = new()
                };
            }

            // --- Business Logic: Determine Performance Tiers ---
            // Base the thresholds on the entire filtered dataset's average
            var avgShareRate = allMetrics.Average(m => m.ShareRatePercentage);
            var avgCompletions = allMetrics.Average(m => m.Completions);

            // Define thresholds based on average performance
            var topThreshold = avgShareRate * 1.5; // 50% better than average share rate
            var lowThreshold = avgShareRate * 0.7; // 30% worse than average share rate

            // --- Assign Performance and Action ---
            var categorizedMetrics = allMetrics.Select(m =>
            {
                // Determine Rating
                if (m.ShareRatePercentage >= topThreshold && m.Completions >= avgCompletions)
                {
                    m.PerformanceRating = "High";
                    m.ActionRecommendation = "Promote";
                }
                else if (m.ShareRatePercentage <= lowThreshold || m.Completions < (avgCompletions * 0.5))
                {
                    m.PerformanceRating = "Low";
                    m.ActionRecommendation = "Replace";
                }
                else
                {
                    m.PerformanceRating = "Medium";
                    m.ActionRecommendation = "Optimize";
                }
                return m;
            }).ToList();

            // 2. Separate into Top/Underperforming
            var topAssets = categorizedMetrics
                .Where(m => m.ActionRecommendation == "Promote")
                .OrderByDescending(m => m.ShareRatePercentage)
                .Take(5)
                .ToList();

            var underperforming = categorizedMetrics
                .Where(m => m.ActionRecommendation == "Replace")
                .OrderBy(m => m.ShareRatePercentage)
                .Take(5)
                .ToList();

            return new ContentEffectivenessDTO
            {
                TopPerformingAssets = topAssets,
                UnderperformingContent = underperforming
            };
        }
    }
}
