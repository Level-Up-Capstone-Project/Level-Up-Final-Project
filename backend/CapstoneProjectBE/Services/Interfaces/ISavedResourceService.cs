using CapstoneProjectBE.DTOs.SavedResources;

namespace CapstoneProjectBE.Services.Interfaces
{
    public interface ISavedResourceService
    {
        Task<IReadOnlyList<SavedResourceDto>> GetAllAsync(Guid userId, CancellationToken cancellationToken);
        Task<SavedResourceDto> SaveAsync(Guid userId, Guid resourceId, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid userId, Guid resourceId, CancellationToken cancellationToken);
    }
}
