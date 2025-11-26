using ClickHealthBackend.DTOs;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClickHealthBackend.Controllers
{
    [ApiController]
    [Route("api/assets")]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetService _assetService;

        public AssetsController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadAsset([FromBody] UploadAssetDto dto)
        {
            if (dto == null) return BadRequest("Invalid payload");
            var created = await _assetService.CreateAssetAsync(dto);
            return CreatedAtAction(nameof(GetAssetById), new { assetId = created.AssetId }, created);
        }

        [HttpGet("{assetId}")]
        public async Task<IActionResult> GetAssetById(string assetId)
        {
            var asset = await _assetService.GetAssetByIdAsync(assetId);
            if (asset == null) return NotFound();
            return Ok(asset);
        }

        [HttpGet("content/{contentId}")]
        public async Task<IActionResult> GetAssetsByContent(string contentId)
        {
            var list = await _assetService.GetAssetsByContentIdAsync(contentId);
            return Ok(list);
        }
    }
}
