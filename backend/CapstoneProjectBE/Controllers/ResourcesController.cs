using Microsoft.AspNetCore.Mvc;
using CapstoneProjectBE.Services.Interfaces;
using CapstoneProjectBE.DTOs.Resources;

namespace CapstoneProjectBE.Controllers
{
    [ApiController]
    [Route("api/resources")]
    public class ResourcesController : ControllerBase
    {
        private readonly ILearningResourceService _resourceService;

        public ResourcesController(ILearningResourceService resourceService)
        {
            _resourceService = resourceService;

        }

        //GET /api/resources/{id}
        [HttpGet("{resourceId:guid}")]
        public async Task<ActionResult<LearningResourceDto>> GetById(Guid resourceId, CancellationToken cancellationToken)
        {
            var resource = await _resourceService.GetByIdAsync(resourceId, cancellationToken);

            if (resource is null)
            {
                return NotFound();
            }
            return Ok(resource);
        }

        //GET /api/resources?skillID=
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<LearningResourceDto>>> GetBySkill([FromQuery] Guid? skillId, CancellationToken cancellationToken)
        {
            if (skillId is null)
            {
                return BadRequest("skillID query parameter is required");
            }

            var resources = await _resourceService.GetBySkillAsync(skillId.Value, cancellationToken);
            return Ok(resources);
        }
    }
}