using CapstoneProjectBE.DTOs.Jobs;

namespace CapstoneProjectBE.Services.Interfaces
{
    public interface IJobMatchService
    {
        //all open jobs, best match first
        Task<List<JobMatchDto>> GetMatchesAsync (Guid userId, CancellationToken cancellationToken);

        //one job's match
        Task<JobMatchDto?> GetMatchForJobAsync (Guid userId, Guid jobId, CancellationToken cancellationToken);

        //recalculate and store matches for every open job
        Task<List<JobMatchDto>> RecalculateAsync(Guid userId, CancellationToken cancellationToken);
    }
}
