using CapstoneProjectBE.DTOs.CVs;
using Microsoft.AspNetCore.Http;

namespace CapstoneProjectBE.Services.Interfaces
{
    /// <summary>
    /// Defines the operations provided by the CV service abstraction.
    /// </summary>
    public interface ICVService
    {
        Task<CVDto> UploadAsync(Guid userId, IFormFile file, CancellationToken cancellationToken);
        Task<IReadOnlyList<CVDto>> GetForUserAsync (Guid userId, CancellationToken cancellationToken);
        Task<CVDto?> GetCurrentAsync(Guid userId, CancellationToken cancellationToken);
        Task<CVDto?> GetByIdAsync(Guid userId, Guid cvId, CancellationToken cancellationToken);
    }
}
