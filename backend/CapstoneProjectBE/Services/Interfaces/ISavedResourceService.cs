using CapstoneProjectBE.DTOs.SavedResources;

namespace CapstoneProjectBE.Services.Interfaces
{
    /// <summary>
    /// Defines the operations provided by the Saved Resource service abstraction.
    /// </summary>
    public interface ISavedResourceService
    {
        Task<IReadOnlyList<SavedResourceDto>> GetAllAsync(Guid userId, CancellationToken cancellationToken);
        Task<SavedResourceDto> SaveAsync(Guid userId, Guid resourceId, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid userId, Guid resourceId, CancellationToken cancellationToken);
    }
}
