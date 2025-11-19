using ClickHealthBackend.DTOs;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using System.Threading.Tasks;

namespace ClickHealthBackend.Services.Interfaces
{
    public interface IContentService
    {
        Task<string> UploadContentAsync(ContentDTO newContentDto, string uploadedByUserId, string uploadedByUserName);
        Task<List<MedicalContentResponseDto>> GetContentsByStatusAsync(ContentStatus status);
        Task<bool> UpdateContentStatusAsync(string contentId, ContentStatus newStatus, string approverName, string notes);
        Task<List<ContentWorkflowDTO>> GetContentWorkflowByUploaderAsync(string uploaderId);
<<<<<<< HEAD
=======
        
>>>>>>> 6d54bde216ffe9ad760fc6fd5b3df6d9b1538c81
    }
}