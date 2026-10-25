using CapstoneProjectBE.Services;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CapstoneProjectBE.DTOs.Common;
using CapstoneProjectBE.DTOs.Jobs;

namespace CapstoneProjectBE.Controllers
{
    /// <summary>
    /// Handles HTTP requests for Jobs operations.
    /// </summary>
    [ApiController]
    [Route("api/jobs")]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        //GET /api/jobs (optional: page, pageSize, careerID, location, jobType, search)
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<JobSummaryDto>>> GetJobs([FromQuery] JobQueryDto query, CancellationToken cancellationToken)
        {
            var jobs = await _jobService.GetJobsAsync(query, cancellationToken);
            return Ok(jobs);
        }

        //GET /api/jobs/{id}
        [HttpGet("{jobId:guid}")]
        public async Task<ActionResult<JobDto>> GetJob(Guid jobId, CancellationToken cancellationToken)
        {
            var job = await _jobService.GetByIdAsync(jobId, cancellationToken);
            
            if (job is null)
            {
                return NotFound();
            }
            return Ok(job);
        }
    }
}
