using CapstoneProjectBE.DTOs.SkillGap;

namespace CapstoneProjectBE.Services.Interfaces
{
    /// <summary>
    /// Defines the operations provided by the Skill Gap service abstraction.
    /// </summary>
    public interface ISkillGapService
    {
        Task<SkillGapDto?> GetSkillGapAsync(Guid userId, CancellationToken cancellationToken);
    }
}
