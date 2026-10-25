using CapstoneProjectBE.DTOs.Skills;

namespace CapstoneProjectBE.Services.Interfaces
{
    /// <summary>
    /// Defines the operations provided by the Skill service abstraction.
    /// </summary>
    public interface ISkillService
    {
        Task<IReadOnlyList<SkillDto>> GetAllAsync(string? category, CancellationToken cancellationToken);
        Task<SkillDto?> GetByIdAsync(Guid skillId,  CancellationToken cancellationToken);
    }
}
