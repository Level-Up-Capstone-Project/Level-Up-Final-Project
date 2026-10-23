using CapstoneProjectBE.DTOs.Jobs;
using CapstoneProjectBE.Services;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CapstoneProjectBE.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/saved-jobs")]
    public class SavedJobsController : ControllerBase
    {
        private readonly ISavedJobService _savedJobService;

        public SavedJobsController(ISavedJobService savedJobService)
        {
            _savedJobService = savedJobService;
        }

        //GET /api/saved-jobs
        [HttpGet]
        public async Task<ActionResult<List<SavedJobDto>>> GetSavedJobs(CancellationToken cancellationToken)
        {
           if (!TryGetUserId (out Guid userId))
            {
                return Unauthorized();
            }

            var savedJobs = await _savedJobService.GetSavedJobsAsync(userId, cancellationToken);
            return Ok(savedJobs);
        }

        //POST /api/saved-jobs/{jobID}
        [HttpPost("{jobId:guid}")]
        public async Task<ActionResult<SavedJobDto>> SaveJob(Guid jobId, CancellationToken cancellationToken)
        {
            if(!TryGetUserId(out Guid userId))
            {
                return Unauthorized();
            }

            var savedJob = await _savedJobService.SaveJobAsync(userId, jobId, cancellationToken);

            if (savedJob is null)
            {
                return NotFound();
            }

            return Ok(savedJob);
        }

        //DELETE api/saved-jobs/{jobID}
        [HttpDelete("{jobId:guid}")]
        public async Task<IActionResult> UnsaveJob(Guid jobId, CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out Guid userId))
            {
                return Unauthorized();
            }

            var removed = await _savedJobService.UnsavedJobAsync(userId, jobId, cancellationToken);

            if (!removed)
            {
                return NotFound();
            }
            return NoContent();
        }

        //reads logged in user's ID from token
        private bool TryGetUserId(out Guid userId)
        {
            return Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
        }
    }
}
