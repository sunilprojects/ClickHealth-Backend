
﻿using ClickHealthBackend.DTOs;
using ClickHealthBackend.Enums;
﻿using ClickHealthBackend.Models;
using System.Threading.Tasks;

namespace ClickHealthBackend.Repositories.Interfaces
{
    public interface IContentRepository
    {
        /// <summary>
        /// Retrieves a single Content document by its unique ID.
        /// </summary>
        /// <param name="id">The ContentId.</param>
        Task<Content> GetByIdAsync(string id);

        /// <summary>
        /// Retrieves all Content documents that are currently in the 'Pending' status, used for the Medical Chief's review queue.
        /// </summary>
        Task<IEnumerable<Content>> GetPendingContentAsync();

        /// <summary>
        /// Retrieves all Content documents in the database.
        /// </summary>
        Task<IEnumerable<Content>> GetAllAsync();

        /// <summary>
        /// Adds a new Content document to the database.
        /// </summary>
        Task CreateAsync(Content content);

        /// <summary>
        /// Updates an existing Content document (used primarily by the Medical Service to change status).
        /// </summary>
        Task<bool> UpdateAsync(Content content);


        Task<List<ContentMetricsDTO>> GetContentMetricsAsync(PerformanceFilterDTO filter);
        Task<List<CityPerformanceDTO>> GetCityPerformanceAsync(PerformanceFilterDTO filter);
        Task<List<Content>> GetAllContentAsync();
        Task<List<Content>> GetApprovedContentAsync();
        // Task<List<Content>> GetManyByIdsAsync(List<string> contentIds);


        Task<bool> UpdateStatusByCustomIdAsync(
    string contentCustomId,
    ContentStatus newStatus,
    string approverCustomId,
    string approverName,
    string? remarks);

    
    Task<List<Content>> GetByIdsAsync(List<string> customIds);

    }




}

