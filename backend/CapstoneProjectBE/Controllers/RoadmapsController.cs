using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CapstoneProjectBE.DTOs.Roadmaps;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CapstoneProjectBE.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace CapstoneProjectBE.Controllers
{
    /// <summary>
    /// Handles HTTP requests for Roadmaps operations.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/roadmaps")]
    public class RoadmapsController : ControllerBase
    {
        private readonly IRoadmapService _roadmapService;

        public RoadmapsController(IRoadmapService roadmapService)
        {
            _roadmapService = roadmapService;
        }

        //GET /api/roadmaps/items/{id}/resources
        [HttpGet("items/{id}/resources")]
        public async Task<ActionResult<List<RoadmapItemResourceDto>>> GetItemResources(Guid id)
        {
            if (!TryGetUserId(out Guid userId))
            {
                return Unauthorized();
            }

            var resources = await _roadmapService.GetItemResourceAsync(userId, id);

            if (resources == null)
            {
                return NotFound();
            }
            return Ok(resources);
        }

        //GET /api/roadmaps/items/{id}
        [HttpGet("items/{id}")]
        public async Task<ActionResult<RoadmapItemDto>> GetItem(Guid id)
        {
            if (!TryGetUserId(out Guid userId))
            {
                return Unauthorized();
            }

            var item = await _roadmapService.GetItemByIdAsync(userId, id);

            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        //GET /api/roadmaps/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<RoadmapDto>> GetRoadmap(Guid id)
        {
            if (!TryGetUserId(out Guid userId))
            {
                return Unauthorized();
            }

            var roadmap = await _roadmapService.GetRoadmapByIdAsync(userId, id);

            if (roadmap == null)
            {
                return NotFound();
            }

            return Ok(roadmap);
        }

        //GET /api/roadmaps
        [HttpGet]
        public async Task<ActionResult<List<RoadmapSummaryDto>>> GetRoadmaps()
        {
            if (!TryGetUserId(out Guid userId))
            {
                return Unauthorized();
            }

            var roadmaps = await _roadmapService.GetRoadmapForUserAsync(userId);
            return Ok(roadmaps);
        }

        //GET /api/roadmaps/current
        [HttpGet("current")]
        public async Task<ActionResult<RoadmapDto>> GetCurrentRoadmap()
        {
            if (!TryGetUserId(out Guid userId))
            {
                return Unauthorized();
            }

            var roadmap = await _roadmapService.GetCurrentRoadmapAsync(userId);

            if (roadmap == null)
            {
                return NotFound();
            }
            return Ok(roadmap);
        }

        //POST /api/roadmaps/items/{itemID}/resources/{resourceID}/complete
        [HttpPost("items/{itemID}/resources/{resourceID}/complete")]
        public async Task<ActionResult<CompleteResourceResultDto>> CompleteResource(Guid itemID, Guid resourceID)
        {
            if (!TryGetUserId(out Guid userId))
            {
                return Unauthorized();
            }

            var result = await _roadmapService.CompleteResourceAsync(userId, itemID, resourceID);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        //POST /api/roadmaps/generate
        [EnableRateLimiting("ai-workflow")]
        [HttpPost("generate")]
        public async Task<ActionResult<RoadmapDto>> GenerateRoadmap(GenerateRoadmapDto request, CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out Guid userId))
            {
                return Unauthorized();
            }

            RoadmapDto? roadmap;
            try
            {
                roadmap = await _roadmapService.GenerateRoadmapAsync(userId, request.CareerID, cancellationToken);
            }
            catch (N8nAnalysisException exception)
            {
                return Problem(title: exception.Message, statusCode: exception.StatusCode);
            }

            if (roadmap == null)
            {
                return NotFound();
            }

            return CreatedAtAction(nameof(GetRoadmap), new { id = roadmap.RoadmapID }, roadmap);
        }

        private bool TryGetUserId(out Guid userId)
        {
            return Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
        }
    }
}
