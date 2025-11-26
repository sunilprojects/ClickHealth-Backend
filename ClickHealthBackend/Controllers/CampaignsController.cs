using ClickHealthBackend.DTOs;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Implementations;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClickHealthBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsController : ControllerBase
    {
        private readonly ICampaignRepository _campaignRepo;
        private readonly ICampaignMetricsService _metricsService;
        private readonly IContentRepository _contentRepository;
        private readonly IMongoCollection<Content> _contentCollection;

        private readonly PatientInviteService _inviteService;


        public CampaignsController(
            ICampaignRepository campaignRepo,
            ICampaignMetricsService metricsService,
            IContentRepository contentRepository,
            IMongoDatabase database,
            PatientInviteService inviteService)
        {
            _campaignRepo = campaignRepo;
            _metricsService = metricsService;
            _contentRepository = contentRepository;
            _contentCollection = database.GetCollection<Content>("Contents");
            _inviteService = inviteService;

        }

        // ---------- Map Entity to DTO ----------
        private CampaignDTO MapToDto(Campaign campaign)
        {
            if (campaign == null) return null;

            Dictionary<string, object> targetMetrics = null;

            if (campaign.TargetMetrics != null)
            {
                targetMetrics = campaign.TargetMetrics.ToDictionary(
                    kvp => kvp.Name,
                    kvp => ConvertBsonValue(kvp.Value)
                );
            }

            return new CampaignDTO
            {
                CampaignId = campaign.CampaignId,
                CampaignCustomId = campaign.CampaignCustomId,
                Name = campaign.Name,
                Therapy = campaign.Therapy,
                Cities = campaign.Cities,
                Territories = campaign.Territories,
                Language = campaign.Language,
                StartDate = campaign.StartDate,
                EndDate = campaign.EndDate,
                CreatedByUserId = campaign.CreatedByUserId,
                CreatedAt = campaign.CreatedAt,
                Status = campaign.Status.ToString(),
                ContentIds = campaign.ContentIds,
                TargetMetrics = targetMetrics
            };
        }

        // ---------- FIX MISSING METHOD ----------
        private object ConvertBsonValue(BsonValue value)
        {
            if (value == null || value.IsBsonNull) return null;

            return value.BsonType switch
            {
                BsonType.String => value.AsString,
                BsonType.Int32 => value.AsInt32,
                BsonType.Int64 => value.AsInt64,
                BsonType.Double => value.AsDouble,
                BsonType.Boolean => value.AsBoolean,
                BsonType.DateTime => value.ToUniversalTime(),
                BsonType.ObjectId => value.AsObjectId.ToString(),
                BsonType.Array => value.AsBsonArray.Select(ConvertBsonValue).ToList(),
                BsonType.Document => value.AsBsonDocument.ToDictionary(
                                        x => x.Name,
                                        x => ConvertBsonValue(x.Value)
                                    ),
                _ => value.ToString()
            };
        }

        private async Task<string> GenerateCampaignCustomId()
        {
            return await _campaignRepo.GenerateCampaignCustomIdAsync();
        }

        // CREATE CAMPAIGN
        [HttpPost]
        public async Task<ActionResult<CampaignDTO>> CreateCampaignAsync([FromBody] CreateCampaignRequest request)
        {
            if (request == null || request.ContentIds == null || request.ContentIds.Count == 0)
                return BadRequest("ContentIds are required.");

            var filter = Builders<Content>.Filter.In(c => c.ContentCustomId, request.ContentIds)
                       & Builders<Content>.Filter.Eq(c => c.Status, ContentStatus.Approved);

            var approvedContents = await _contentCollection.Find(filter).ToListAsync();

            if (approvedContents.Count != request.ContentIds.Count)
                return BadRequest("Some content items are not approved.");

            var campaign = new Campaign
            {
                CampaignCustomId = await GenerateCampaignCustomId(),
                Name = request.Name,
                Therapy = request.Therapy,
                Language = request.Language,
                Cities = request.Cities,
                Territories = request.Territories,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = CampaignStatus.Active,
                CreatedByUserId = request.CreatedByUserId,
                ContentIds = request.ContentIds,
                CreatedAt = DateTime.UtcNow
            };

            await _campaignRepo.CreateCampaignAsync(campaign);

            return CreatedAtAction(nameof(GetCampaign),
                new { campaignCustomId = campaign.CampaignCustomId },
                MapToDto(campaign));
        }

        // GET ALL
        [HttpGet("Fetch")]
        public async Task<ActionResult<List<CampaignDTO>>> GetAllCampaigns()
        {
            var campaigns = await _campaignRepo.GetAllCampaignsAsync();
            return Ok(campaigns.Select(MapToDto).ToList());
        }

        // GET ONE
        [HttpGet("{campaignCustomId}")]
        public async Task<ActionResult<CampaignDTO>> GetCampaign(string campaignCustomId)
        {
            var campaign = await _campaignRepo.GetCampaignByIdAsync(campaignCustomId);
            if (campaign == null)
                return NotFound($"Campaign '{campaignCustomId}' not found.");

            return Ok(MapToDto(campaign));
        }

        // HEATMAP
        [HttpGet("{campaignId}/regional-performance-heatmap")]
        public async Task<IActionResult> GetRegionalPerformanceHeatMap(string campaignId)
        {
            var heatmapData = await _metricsService.GetRegionalPerformanceHeatMapAsync(campaignId);
            if (heatmapData == null || heatmapData.Count == 0)
                return NotFound($"No regional metrics available for campaign {campaignId}");

            return Ok(heatmapData);
        }

        [HttpPost("{campaignId}/send-to-patients")]
        public async Task<IActionResult> SendCampaign(string campaignId, [FromQuery] string hcpId, [FromQuery] string specialty)
        {
            await _inviteService.SendCampaignToPatientsAsync(campaignId, hcpId, specialty);
            return Ok(new { message = "Campaign sent to patients successfully." });
        }
    }
}
