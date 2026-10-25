using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CapstoneProjectBE.DTOs.SkillGap;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace CapstoneProjectBE.Controllers
{
    /// <summary>
    /// Handles HTTP requests for Skill Gap operations.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/skill-gap")]
    public class SkillGapController : ControllerBase
    {
        private readonly ISkillGapService _skillGapService;

        public SkillGapController(ISkillGapService skillGapService)
        {
            _skillGapService = skillGapService;
        }

        [HttpGet]
        public async Task<ActionResult<SkillGapDto>> GetSkillGap(CancellationToken cancellationToken)
        {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized();
            }

            SkillGapDto? skillGap = await _skillGapService.GetSkillGapAsync(userId, cancellationToken);

            if(skillGap is  null)
            {
                return NotFound(new
                {
                    message = "A profile with a valid target career is required."
                });
            }
            return Ok(skillGap);
        }
    }
}
