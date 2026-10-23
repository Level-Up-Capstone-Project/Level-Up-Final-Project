using CapstoneProjectBE.DTOs.Skills;

namespace CapstoneProjectBE.Services.Interfaces
{
    public interface ISkillService
    {
        Task<IReadOnlyList<SkillDto>> GetAllAsync(string? category, CancellationToken cancellationToken);
        Task<SkillDto?> GetByIdAsync(Guid skillId,  CancellationToken cancellationToken);
    }
}
