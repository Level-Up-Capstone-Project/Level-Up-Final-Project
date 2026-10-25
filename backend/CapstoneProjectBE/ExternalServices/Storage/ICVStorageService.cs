using Microsoft.AspNetCore.Http;

namespace CapstoneProjectBE.ExternalServices.Storage
{
    /// <summary>
    /// Defines the contract for the CVStorage Service integration.
    /// </summary>
    public interface ICVStorageService
    {
        Task<string> UploadAsync(Guid userId, Guid cvId, IFormFile file, CancellationToken cancellationToken);
        Task DeleteAsync(string objPath, CancellationToken cancellationToken);
        Task<byte[]> DownloadAsync(string objectPath, CancellationToken cancellationToken);
    }
}
