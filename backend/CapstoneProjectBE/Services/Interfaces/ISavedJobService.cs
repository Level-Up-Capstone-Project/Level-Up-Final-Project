using CapstoneProjectBE.DTOs.Jobs;

namespace CapstoneProjectBE.Services.Interfaces
{
    /// <summary>
    /// Defines the operations provided by the Saved Job service abstraction.
    /// </summary>
    public interface ISavedJobService
    {
        Task<List<SavedJobDto>> GetSavedJobsAsync (Guid userId, CancellationToken cancellationToken);

        //null if job does not exist
        Task<SavedJobDto?> SaveJobAsync (Guid userId, Guid jobId, CancellationToken cancellationToken);

        //false if job not saved
        Task<bool> UnsavedJobAsync (Guid userId, Guid jobId, CancellationToken cancellationToken);
    }
}
