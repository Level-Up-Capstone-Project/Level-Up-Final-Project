using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CapstoneProjectBE.DTOs.Jobs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CapstoneProjectBE.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/job-matches")]
    public class JobMatchesController : ControllerBase
    {

        private readonly IJobMatchService _jobMatchService;

        public JobMatchesController(IJobMatchService jobMatchService)
        {
            _jobMatchService = jobMatchService;
        }

        //GET /api/job-matches
        [HttpGet]
        public async Task<ActionResult<List<JobMatchDto>>> GetMatches(CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out Guid userId))
            {
                return Unauthorized();
            }

            var matches = await _jobMatchService.RecalculateAsync(userId, cancellationToken);
            return Ok(matches);
        }

        //GET /api/job-matches/{jobID}
        [HttpGet("{jobId:guid}")]
        public async Task<ActionResult<JobMatchDto>> GetMatch(Guid jobId, CancellationToken cancellationToken)
        {

            if (!TryGetUserId(out Guid userId))
            {
                return Unauthorized();
            }

            var match = await _jobMatchService.GetMatchForJobAsync(userId, jobId, cancellationToken);

            if(match is null)
            {
                return NotFound();
            }

            return Ok(match);
        }

        //POST /api/job-matches/recalculate
        [HttpPost("recalculate")]
        public async Task<ActionResult<List<JobMatchDto>>> Recalculate(CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out Guid userId))
            {
                return Unauthorized();
            }

            var matches = await _jobMatchService.GetMatchesAsync(userId, cancellationToken);
            return Ok(matches);
        }

        private bool TryGetUserId(out Guid userId)
        {
            return Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
        }


    }
}
