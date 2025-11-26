using ClickHealthBackend.DTOs;
using ClickHealthBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.Services.Interfaces
{
    public interface IHCPService
    {
        Task<HCPResponseDto> CreateHCPAsync(HCPCreateDto dto);
        Task<List<HCPResponseDto>> GetAllHCPsAsync();
        Task<HCPResponseDto> GetHCPByIdAsync(string hcpId);
    }
}
