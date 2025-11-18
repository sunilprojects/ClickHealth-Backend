using ClickHealthBackend.DTOs;
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

        public CampaignsController(ICampaignRepository campaignRepo, ICampaignMetricsService metricsService)
        {
            _campaignRepo = campaignRepo;
            _metricsService = metricsService;
        }

        // --- DTO Mapping Helper (Maps DB Model to Clean DTO) ---
        private CampaignDTO MapToDto(Campaign campaign)
        {
            if (campaign == null) return null;

            // Convert BsonDocument to Dictionary<string, object>
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

        // ✅ Helper: Convert BsonValue to C# object safely
        private object ConvertBsonValue(BsonValue value)
        {
            if (value == null || value.IsBsonNull) return null;

            switch (value.BsonType)
            {
                case BsonType.String:
                    return value.AsString;
                case BsonType.Int32:
                    return value.AsInt32;
                case BsonType.Int64:
                    return value.AsInt64;
                case BsonType.Double:
                    return value.AsDouble;
                case BsonType.Boolean:
                    return value.AsBoolean;
                case BsonType.DateTime:
                    return value.ToUniversalTime();
                case BsonType.ObjectId:
                    return value.AsObjectId.ToString();
                case BsonType.Array:
                    return value.AsBsonArray.Select(ConvertBsonValue).ToList();
                case BsonType.Document:
                    return value.AsBsonDocument.ToDictionary(e => e.Name, e => ConvertBsonValue(e.Value));
                default:
                    return value.ToString();
            }
        }
        private async Task<string> GenerateCampaignCustomId()
        {
            return await _campaignRepo.GenerateCampaignCustomIdAsync();
        }


        // --- Create Campaign ---
        [HttpPost]
        public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignDTO campaignDto)
        {
            // Get User ID (best practice)
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                // Placeholder for unauthenticated users
                userId = ObjectId.GenerateNewId().ToString();
            }

            // Convert TargetMetrics DTO (Dictionary<string, object>) to BsonDocument
            var bsonTargetMetrics = new BsonDocument();
            if (campaignDto.TargetMetrics != null)
            {
                foreach (var kvp in campaignDto.TargetMetrics)
                {
                    // Handle JsonElement (from System.Text.Json deserialization)
                    if (kvp.Value is JsonElement jsonElement)
                    {
                        BsonValue bsonValue = ConvertJsonElementToBsonValue(jsonElement);
                        bsonTargetMetrics.Add(kvp.Key, bsonValue);
                    }
                    else if (kvp.Value != null)
                    {
                        bsonTargetMetrics.Add(kvp.Key, BsonValue.Create(kvp.Value));
                    }
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

            return CreatedAtAction(nameof(GetCampaign), new { id = createdCampaign.CampaignId }, MapToDto(createdCampaign));
        }

        // --- Get All Campaigns ---
        [HttpGet("Fetch")]
        public async Task<ActionResult<List<CampaignDTO>>> GetAllCampaigns()
        {
            var campaigns = await _campaignRepo.GetAllCampaignsAsync();
            var campaignDtos = campaigns.Select(MapToDto).ToList();
            return Ok(campaignDtos);
        }

        // --- Get Campaign by ID ---
        [HttpGet("{id}")]
        public async Task<ActionResult<CampaignDTO>> GetCampaign(string id)
        {
            var campaign = await _campaignRepo.GetCampaignByIdAsync(id);
            if (campaign == null)
                return NotFound();

            return Ok(MapToDto(campaign));
        }

        // --- Territory Regional Performance HeatMap API ---
        [HttpGet("{campaignId}/regional-performance-heatmap")]
        public async Task<IActionResult> GetRegionalPerformanceHeatMap(string campaignId)
        {
            var heatmapData = await _metricsService.GetRegionalPerformanceHeatMapAsync(campaignId);
            if (heatmapData == null || heatmapData.Count == 0)
            {
                return NotFound($"No metrics found for campaign ID: {campaignId}");
            }
            return Ok(heatmapData);
        }

        // --- Helper: Convert JsonElement to BsonValue ---
        private BsonValue ConvertJsonElementToBsonValue(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    return new BsonString(element.GetString());
                case JsonValueKind.Number:
                    if (element.TryGetInt32(out int intValue)) return new BsonInt32(intValue);
                    if (element.TryGetInt64(out long longValue)) return new BsonInt64(longValue);
                    return new BsonDouble(element.GetDouble());
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return new BsonBoolean(element.GetBoolean());
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return BsonNull.Value;
                case JsonValueKind.Object:
                    var doc = new BsonDocument();
                    foreach (var property in element.EnumerateObject())
                    {
                        doc.Add(property.Name, ConvertJsonElementToBsonValue(property.Value));
                    }
                    return doc;
                case JsonValueKind.Array:
                    var arr = new BsonArray();
                    foreach (var item in element.EnumerateArray())
                    {
                        arr.Add(ConvertJsonElementToBsonValue(item));
                    }
                    return arr;
                default:
                    return BsonNull.Value;
            }
        }
    }
}
