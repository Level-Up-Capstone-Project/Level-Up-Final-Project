using CapstoneProjectBE.DTOs.CVs;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using CapstoneProjectBE.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace CapstoneProjectBE.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/cvs")]
    public class CVsController : ControllerBase
    {
        private readonly ICVService _cvService;
        private readonly ICVAnalysisService _cvAnalysisService;

        public CVsController(ICVService cvService, ICVAnalysisService cVAnalysisService)
        {
            _cvService = cvService;
            _cvAnalysisService = cVAnalysisService;
        }

        [EnableRateLimiting("file-upload")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(6*1024*1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 6*1024*1024)]
        public async Task<ActionResult<CVDto>> Upload(IFormFile file, CancellationToken cancellationToken)
        {
            if(!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }

            try
            {
                CVDto cv = await _cvService.UploadAsync(userId, file, cancellationToken);

                return CreatedAtAction(nameof(GetById), new { cvID = cv.CVID }, cv);
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch(KeyNotFoundException)
            {
                return Unauthorized();
            }
            catch(HttpRequestException)
            {
                return Problem(title: "CV storage currently unavailable.", statusCode: StatusCodes.Status502BadGateway);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CVDto>>> GetAll(CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }

            IReadOnlyList<CVDto> cv = await _cvService.GetForUserAsync(userId, cancellationToken);

            return Ok(cv);
        }

        [HttpGet("current")]
        public async Task<ActionResult<CVDto>> GetCurrent(CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }

            CVDto? cv = await _cvService.GetCurrentAsync(userId,cancellationToken);

            if(cv is null)
            {
                return NotFound(new
                {
                    message = "A current CV could not be found."
                });
            }
            return Ok(cv);
        }

        [HttpGet("{cvID:guid}")]
        public async Task<ActionResult<CVDto>> GetById(Guid cvID, CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }

            CVDto? cv = await _cvService.GetByIdAsync(userId, cvID, cancellationToken);

            if(cv is null)
            {
                return NotFound(new
                {
                    message = "CV could not be found."
                });
            }
            return Ok(cv);
        }

        [EnableRateLimiting("ai-workflow")]
        [HttpPost("{cvId:guid}/analyse")]
        public async Task<ActionResult<CVAnalysisDto>> Analyse(Guid cvId, CancellationToken cancellationToken)
        {
            if (Request.Headers.Accept.ToString().Contains("application/x-ndjson", StringComparison.OrdinalIgnoreCase))
            {
                await AnalysisStream.WriteAsync(HttpContext, async progress =>
                {
                    var response = await AnalyseCore(cvId, cancellationToken, progress);
                    return response.Result ?? Ok(response.Value);
                });
                return new EmptyResult();
            }
            return await AnalyseCore(cvId, cancellationToken);
        }

        private async Task<ActionResult<CVAnalysisDto>> AnalyseCore(Guid cvId, CancellationToken cancellationToken, Func<int, Task>? progress = null)
        {
            if (!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }

            try
            {
                CVAnalysisDto? analysis = await _cvAnalysisService.AnalyseAsync(userId, cvId, cancellationToken, progress);

                if (analysis is null)
                {
                    return NotFound(new
                    {
                        message = "The CV could not be found."
                    });
                }

                return StatusCode(StatusCodes.Status201Created, analysis);
            }
            catch (N8nAnalysisException error)
            {
                var problem = new ProblemDetails { Title = error.Message, Status = error.StatusCode };
                if (error.UpstreamStatusCode is int upstreamStatusCode)
                    problem.Extensions["upstreamStatusCode"] = upstreamStatusCode;
                return StatusCode(error.StatusCode, problem);
            }
            catch(InvalidDataException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch(FileNotFoundException)
            {
                return NotFound(new
                {
                    message = "The stored CV file could not be found."
                });
            }
            catch(HttpRequestException)
            {
                return Problem(title: "CV storage is currently unavailable.", statusCode: StatusCodes.Status502BadGateway);
            }
        }

        [HttpGet("{cvId:guid}/analyses")]
        public async Task<ActionResult<IReadOnlyList<CVAnalysisDto>>> GetAnalyses(Guid cvId, CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }

            IReadOnlyList<CVAnalysisDto>? analyses = await _cvAnalysisService.GetForCVAsync(userId, cvId, cancellationToken);

            if (analyses is null)
            {
                return NotFound(new
                {
                    message = "The CV could not be found."
                });
            }

            return Ok(analyses);
        }

        [HttpGet("current/analysis")]
        public async Task<ActionResult<CVAnalysisDto>> GetCurrentAnalysis(CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }

            CVAnalysisDto? analysis = await _cvAnalysisService.GetCurrentAsync(userId, cancellationToken);

            if (analysis is null)
            {
                return NotFound(new
                {
                    message = "The current CV has not been analysed."
                });
            }
            return Ok(analysis);
        }

        private bool TryGetCurrentUserId(out Guid userId)
        {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userIdClaim, out userId);
        }
    }
}
