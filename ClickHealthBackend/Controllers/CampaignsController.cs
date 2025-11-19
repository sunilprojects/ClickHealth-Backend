using ClickHealthBackend.DTOs;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
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
        private readonly IMongoCollection<Content> _content;

        public CampaignsController(
            ICampaignRepository campaignRepo,
            ICampaignMetricsService metricsService,
            IContentRepository contentRepo)
        {
            _campaignRepo = campaignRepo;
            _metricsService = metricsService;
            _contentRepository = contentRepo;

            // optional: if needed for content validation
            _content = _contentRepository?.GetContentCollection();
        }

        // --- DTO Mapping Helper ---
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
                TargetMetrics = targetMetrics
            };
        }

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
                BsonType.Document => value.AsBsonDocument.ToDictionary(e => e.Name, e => ConvertBsonValue(e.Value)),
                _ => value.ToString(),
            };
        }

        private async Task<string> GenerateCampaignCustomId()
        {
            return await _campaignRepo.GenerateCampaignCustomIdAsync();
        }

        // --------------------------------------------------------------------------------
        // ✔ Version 1: Create Campaign (Divya’s version — validates content approval)
        // --------------------------------------------------------------------------------
        [HttpPost("create-v2")]
        public async Task<ActionResult<Campaign>> CreateCampaignAsync(CreateCampaignRequest request)
        {
            var filter = Builders<Content>.Filter.In(c => c.ContentId, request.ContentIds)
                        & Builders<Content>.Filter.Eq(c => c.Status, ContentStatus.Approved);

            var approvedContents = await _content.Find(filter).ToListAsync();

            if (approvedContents.Count != request.ContentIds.Count)
                return BadRequest("Some selected content items are not approved!");

            Campaign campaign = new Campaign
            {
                Name = request.Name,
                Therapy = request.Therapy,
                Language = request.Language,
                Cities = request.Cities,
                Territories = request.Territories,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = CampaignStatus.Active,
                CreatedByUserId = request.CreatedByUserId,
                ContentIds = request.ContentIds
            };

            var created = await _campaignRepo.CreateCampaignAsync(campaign);
            return Ok(created);
        }

        // --------------------------------------------------------------------------------
        // ✔ Version 2: Original Create Campaign (with TargetMetrics support)
        // --------------------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignDTO campaignDto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? ObjectId.GenerateNewId().ToString();

            var bsonTargetMetrics = new BsonDocument();
            if (campaignDto.TargetMetrics != null)
            {
                foreach (var kvp in campaignDto.TargetMetrics)
                {
                    if (kvp.Value is JsonElement jsonElement)
                        bsonTargetMetrics.Add(kvp.Key, ConvertJsonElementToBsonValue(jsonElement));
                    else if (kvp.Value != null)
                        bsonTargetMetrics.Add(kvp.Key, BsonValue.Create(kvp.Value));
                }
            }

            var newCampaign = new Campaign
            {
                CampaignCustomId = await GenerateCampaignCustomId(),
                Name = campaignDto.Name,
                Therapy = campaignDto.Therapy,
                Cities = campaignDto.Cities,
                Territories = campaignDto.Territories,
                Language = campaignDto.Language,
                StartDate = campaignDto.StartDate,
                EndDate = campaignDto.EndDate,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId,
                TargetMetrics = bsonTargetMetrics
            };

            var createdCampaign = await _campaignRepo.CreateCampaignAsync(newCampaign);

            return CreatedAtAction(nameof(GetCampaign),
                new { id = createdCampaign.CampaignId },
                MapToDto(createdCampaign));
        }

        // --- Get All Campaigns ---
        [HttpGet("Fetch")]
        public async Task<ActionResult<List<CampaignDTO>>> GetAllCampaigns()
        {
            var campaigns = await _campaignRepo.GetAllCampaignsAsync();
            return Ok(campaigns.Select(MapToDto).ToList());
        }

        // --- Get Campaign by ID ---
        [HttpGet("{id}")]
        public async Task<ActionResult<CampaignDTO>> GetCampaign(string id)
        {
            var campaign = await _campaignRepo.GetCampaignByIdAsync(id);
            return campaign == null ? NotFound() : Ok(MapToDto(campaign));
        }

        // --- Territory HeatMap Metrics ---
        [HttpGet("{campaignId}/regional-performance-heatmap")]
        public async Task<IActionResult> GetRegionalPerformanceHeatMap(string campaignId)
        {
            var heatmapData = await _metricsService.GetRegionalPerformanceHeatMapAsync(campaignId);

            if (heatmapData == null || heatmapData.Count == 0)
                return NotFound($"No metrics found for campaign ID: {campaignId}");

            return Ok(heatmapData);
        }

        // Convert JsonElement → BsonValue
        private BsonValue ConvertJsonElementToBsonValue(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => new BsonString(element.GetString()),
                JsonValueKind.Number =>
                    element.TryGetInt32(out int i) ? new BsonInt32(i) :
                    element.TryGetInt64(out long l) ? new BsonInt64(l) :
                    new BsonDouble(element.GetDouble()),
                JsonValueKind.True => new BsonBoolean(true),
                JsonValueKind.False => new BsonBoolean(false),
                JsonValueKind.Null => BsonNull.Value,
                JsonValueKind.Undefined => BsonNull.Value,
                JsonValueKind.Object => new BsonDocument(
                    element.EnumerateObject().ToDictionary(
                        p => p.Name,
                        p => ConvertJsonElementToBsonValue(p.Value))),
                JsonValueKind.Array => new BsonArray(
                    element.EnumerateArray().Select(ConvertJsonElementToBsonValue)),
                _ => BsonNull.Value,
            };
        }
    }
}
