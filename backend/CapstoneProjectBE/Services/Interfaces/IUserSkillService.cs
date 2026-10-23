using CapstoneProjectBE.DTOs.Skills;

namespace CapstoneProjectBE.Services.Interfaces
{
    public interface IUserSkillService
    {
        Task<IReadOnlyList<UserSkillDto>> GetForUserAsync(Guid userId, CancellationToken cancellationToken);
        Task<UserSkillDto?> AddSelfReportedAsync(Guid userId, Guid skillId, CancellationToken cancellationToken);
        Task<bool> RemoveSelfReportedAsync(Guid userId, Guid skillId, CancellationToken cancellationToken);
    }
}
