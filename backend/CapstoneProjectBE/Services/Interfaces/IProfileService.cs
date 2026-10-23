using CapstoneProjectBE.DTOs.Profile;

namespace CapstoneProjectBE.Services.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileDto?> GetProfileAsync(Guid userId, CancellationToken cancellationToken);
        Task<ProfileDto?> UpsertProfileAsync(Guid userId, UpdateProfileDto request, CancellationToken cancellationToken);
        Task<ProfileDto?> UpdatePreferencesAsync(Guid userId, UpdateProfilePreferencesDto request, CancellationToken cancellationToken);
        Task<ProfileDto?> UpdateTargetCareerAsync(Guid userId, Guid careerId, CancellationToken cancellationToken);
    }
}
