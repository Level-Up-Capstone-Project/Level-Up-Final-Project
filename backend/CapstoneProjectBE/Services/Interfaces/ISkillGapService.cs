using CapstoneProjectBE.DTOs.SkillGap;

namespace CapstoneProjectBE.Services.Interfaces
{
    public interface ISkillGapService
    {
        Task<SkillGapDto?> GetSkillGapAsync(Guid userId, CancellationToken cancellationToken);
    }
}
