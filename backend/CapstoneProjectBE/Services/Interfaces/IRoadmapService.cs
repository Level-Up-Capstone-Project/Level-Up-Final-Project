using CapstoneProjectBE.DTOs.Roadmaps;

namespace CapstoneProjectBE.Services.Interfaces
{
    /// <summary>
    /// Defines the operations provided by the Roadmap service abstraction.
    /// </summary>
    public interface IRoadmapService
    {

        //resources for a roadmap step
        //null if the step does not exist
        Task<List<RoadmapItemResourceDto>?> GetItemResourceAsync(Guid userID, Guid roadmapItemID);

        //one step by its roadmap_item_id
        //null if not found
        Task<RoadmapItemDto?> GetItemByIdAsync(Guid userID, Guid roadmapItemID);

        //full roadmap by its roadmap_id, including all its steps
        //null if not found
        Task<RoadmapDto?> GetRoadmapByIdAsync(Guid userID, Guid roadmapID);

        //all roadmaps for a user, newest first
        Task<List<RoadmapSummaryDto>> GetRoadmapForUserAsync (Guid userID);

        //user's most recently updated roadmap with steps
        Task<RoadmapDto?> GetCurrentRoadmapAsync (Guid userID);

        //marks resource as complete, updates step status and progress
        Task<CompleteResourceResultDto?> CompleteResourceAsync(Guid userID, Guid roadmapItemID, Guid resourceID);

        //creates roadmap from career's skills
        Task<RoadmapDto?> GenerateRoadmapAsync (Guid userID, Guid careerID, CancellationToken cancellationToken);
    }
}
