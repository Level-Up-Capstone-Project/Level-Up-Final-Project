using CapstoneProjectBE.DTOs.CVs;

namespace CapstoneProjectBE.Services.Interfaces
{
    public interface ICVAnalysisService
    {
        Task<CVAnalysisDto?> AnalyseAsync(Guid userId, Guid cvId, CancellationToken cancellationToken, Func<int, Task>? reportProgress = null);
        Task<IReadOnlyList<CVAnalysisDto>?> GetForCVAsync(Guid userId, Guid cvId, CancellationToken cancellationToken);
        Task<CVAnalysisDto?> GetCurrentAsync(Guid userId, CancellationToken cancellationToken);
    }
}
