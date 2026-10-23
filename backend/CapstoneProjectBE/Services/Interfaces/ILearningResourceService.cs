using CapstoneProjectBE.DTOs.Resources;

namespace CapstoneProjectBE.Services.Interfaces
{
    public interface ILearningResourceService
    {
        //gets one active learning resource by its ID
        Task<LearningResourceDto?> GetByIdAsync (Guid resourceId, CancellationToken cancellationToken);

        //gets all active learning resources that teach specific skill
        Task<IReadOnlyList<LearningResourceDto>> GetBySkillAsync(Guid skillId, CancellationToken cancellationToken);
    }
}