using CapstoneProjectBE.DTOs.Common;
using CapstoneProjectBE.DTOs.Jobs;

namespace CapstoneProjectBE.Services.Interfaces
{
    public interface IJobService
    {
        Task<PagedResultDto<JobSummaryDto>> GetJobsAsync(JobQueryDto query, CancellationToken cancellationToken);
        Task<JobDto?> GetByIdAsync(Guid jobId, CancellationToken cancellationToken);
    }
}