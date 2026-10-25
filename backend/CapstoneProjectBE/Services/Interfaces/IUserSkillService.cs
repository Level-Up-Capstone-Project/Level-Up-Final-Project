using CapstoneProjectBE.DTOs.Skills;

namespace CapstoneProjectBE.Services.Interfaces
{
    /// <summary>
    /// Defines the operations provided by the User Skill service abstraction.
    /// </summary>
    public interface IUserSkillService
    {
        Task<IReadOnlyList<UserSkillDto>> GetForUserAsync(Guid userId, CancellationToken cancellationToken);
        Task<UserSkillDto?> AddSelfReportedAsync(Guid userId, Guid skillId, CancellationToken cancellationToken);
        Task<bool> RemoveSelfReportedAsync(Guid userId, Guid skillId, CancellationToken cancellationToken);
    }
}
