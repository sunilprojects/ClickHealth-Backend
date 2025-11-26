using ClickHealthBackend.DTOs;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClickHealthBackend.Services.Implementations
{
    public class HCPService : IHCPService
    {
        private readonly IHCPRepository _hcpRepo;

        public HCPService(IHCPRepository hcpRepo)
        {
            _hcpRepo = hcpRepo;
        }

        public async Task<HCPResponseDto> CreateHCPAsync(HCPCreateDto dto)
        {
            var hcp = new HCP
            {
                HcpId = dto.HcpId ?? Guid.NewGuid().ToString(),
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Specialty = dto.Specialty,
                City = dto.City,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _hcpRepo.CreateHCPAsync(hcp);

            return new HCPResponseDto
            {
                HcpId = hcp.HcpId,
                Name = hcp.Name,
                Email = hcp.Email,
                Specialty = hcp.Specialty,
                City = hcp.City
            };
        }

        public async Task<List<HCPResponseDto>> GetAllHCPsAsync()
        {
            var hcps = await _hcpRepo.GetAllHCPsAsync();
            return hcps.Select(h => new HCPResponseDto
            {
                HcpId = h.HcpId,
                Name = h.Name,
                Email = h.Email,
                Specialty = h.Specialty,
                City = h.City
            }).ToList();
        }

        public async Task<HCPResponseDto> GetHCPByIdAsync(string hcpId)
        {
            var hcp = await _hcpRepo.GetHCPByIdAsync(hcpId);
            if (hcp == null) return null;

            return new HCPResponseDto
            {
                HcpId = hcp.HcpId,
                Name = hcp.Name,
                Email = hcp.Email,
                Specialty = hcp.Specialty,
                City = hcp.City
            };
        }
    }
}
