using ClickHealthBackend.DTOs;
using ClickHealthBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.Services.Interfaces
{
    public interface IAssetService
    {
        Task<string> GenerateAssetIdAsync();
        Task<ContentAsset> CreateAssetAsync(UploadAssetDto dto, string createdByUserId = null);
        Task<ContentAsset> GetAssetByIdAsync(string assetId);
        Task<List<ContentAsset>> GetAssetsByContentIdAsync(string contentId);
    }
}
