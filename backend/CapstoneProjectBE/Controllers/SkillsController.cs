using CapstoneProjectBE.DTOs.Skills;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProjectBE.Controllers
{
    /// <summary>
    /// Handles HTTP requests for Skills operations.
    /// </summary>
    [ApiController]
    [Route("api/skills")]
    public class SkillsController : ControllerBase
    {
        private readonly ISkillService _skillService;

        public SkillsController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<SkillDto>>> GetAll([FromBody] string? category, CancellationToken cancellationToken)
        {
            IReadOnlyList<SkillDto> skills = await _skillService.GetAllAsync(category, cancellationToken);

            return Ok(skills);
        }

        [HttpGet("{skillId:guid}")]
        public async Task<ActionResult<SkillDto>> GetById(Guid skillId, CancellationToken cancellationToken)
        {
            SkillDto? skill = await _skillService.GetByIdAsync(skillId, cancellationToken);

            if(skill is null)
            {
                return NotFound(new
                {
                    message = "Skill cannot be found."
                });
            }
            return Ok(skill);
        }
    }
}
