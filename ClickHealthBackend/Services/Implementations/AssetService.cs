using ClickHealthBackend.Data;
using ClickHealthBackend.DTOs;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.Services.Implementations
{
    public class AssetService : IAssetService
    {
        private readonly IMongoCollection<ContentAsset> _assetCollection;

        public AssetService(MongoDbContext context)
        {
            _assetCollection = context.ContentAssets;
        }

        public async Task<string> GenerateAssetIdAsync()
        {
            var sort = Builders<ContentAsset>.Sort.Descending(x => x.AssetId);
            var last = await _assetCollection.Find(_ => true).Sort(sort).FirstOrDefaultAsync();

            int next = 1;
            if (last != null && !string.IsNullOrEmpty(last.AssetId))
            {
                var numeric = last.AssetId.Replace("AS", "");
                if (int.TryParse(numeric, out var n))
                    next = n + 1;
            }

            return $"AS{next:D3}";
        }

        public async Task<ContentAsset> CreateAssetAsync(UploadAssetDto dto, string createdByUserId = null)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var asset = new ContentAsset
            {
                AssetId = await GenerateAssetIdAsync(),
                ContentId = dto.ContentId,
                AssetType = Enum.TryParse<AssetType>(dto.AssetType, true, out var t) ? t : AssetType.PDF,
                Url = dto.Url,  
                Title = dto.Title,
                Language = dto.Language,
                CreatedAt = DateTime.UtcNow
            };

            await _assetCollection.InsertOneAsync(asset);
            return asset;
        }

        public async Task<ContentAsset> GetAssetByIdAsync(string assetId)
        {
            return await _assetCollection.Find(a => a.AssetId == assetId).FirstOrDefaultAsync();
        }

        public async Task<List<ContentAsset>> GetAssetsByContentIdAsync(string contentId)
        {
            return await _assetCollection.Find(a => a.ContentId == contentId).ToListAsync();
        }
    }
}

