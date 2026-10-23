using System.Security.Claims;
using CapstoneProjectBE.DTOs.Assessments;
using CapstoneProjectBE.Services.Interfaces;
using CapstoneProjectBE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CapstoneProjectBE.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/assessments")]
    public class AssessmentsController : ControllerBase
    {
        private readonly IAssessmentService _assessmentService;

        public AssessmentsController(IAssessmentService assessmentService)
        {
            _assessmentService = assessmentService;
        }

        [EnableRateLimiting("ai-workflow")]
        [HttpPost]
        public async Task<ActionResult<AssessmentDto>> Start(
            [FromBody] StartAssessmentDto request,
            CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }

            try
            {
                AssessmentDto assessment = await _assessmentService
                    .StartAsync(userId, request, cancellationToken);

                return CreatedAtAction(
                    nameof(GetById),
                    new { skillAssessmentId = assessment.SkillAssessmentID },
                    assessment);
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(new { message = exception.Message });
            }
            catch (N8nAnalysisException exception)
            {
                return Problem(title: exception.Message, statusCode: exception.StatusCode);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }

        [HttpGet("{skillAssessmentId:guid}")]
        public async Task<ActionResult<AssessmentDto>> GetById(
            Guid skillAssessmentId,
            CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }

            AssessmentDto? assessment = await _assessmentService
                .GetByIdAsync(userId, skillAssessmentId, cancellationToken);

            return assessment is null ? NotFound() : Ok(assessment);
        }

        [HttpPost("{skillAssessmentId:guid}/submit")]
        public async Task<ActionResult<AssessmentResultDto>> Submit(
            Guid skillAssessmentId,
            [FromBody] SubmitAssessmentDto request,
            CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }

            try
            {
                AssessmentResultDto? result = await _assessmentService
                    .SubmitAsync(userId, skillAssessmentId, request, cancellationToken);

                return result is null
                    ? NotFound(new { message = "The assessment could not be found." })
                    : Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(new { message = exception.Message });
            }
        }

        [HttpGet("skill/{skillId:guid}")]
        public async Task<ActionResult<IReadOnlyList<AssessmentHistoryDto>>> GetHistory(
            Guid skillId,
            CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }

            IReadOnlyList<AssessmentHistoryDto> history = await _assessmentService
                .GetHistoryAsync(userId, skillId, cancellationToken);

            return Ok(history);
        }

        private bool TryGetCurrentUserId(out Guid userId)
        {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out userId);
        }
    }
}
