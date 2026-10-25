using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CapstoneProjectBE.DTOs.Skills;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace CapstoneProjectBE.Controllers
{
    /// <summary>
    /// Handles HTTP requests for Profile Skills operations.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/profile/skills")]
    public class ProfileSkillsController : ControllerBase
    {
        private readonly IUserSkillService _userSkillService;

        public ProfileSkillsController(IUserSkillService userSkillService)
        {
            _userSkillService = userSkillService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserSkillDto>>> GetSkills(CancellationToken cancellationToken)
        {
            if(!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }

            IReadOnlyList<UserSkillDto> skills = await _userSkillService.GetForUserAsync(userId, cancellationToken);
            return Ok(skills);
        }

        [HttpPost]
        public async Task<ActionResult<UserSkillDto>> AddSkill([FromBody] AddUserSkillDto request, CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }   

            if(!request.SkillID.HasValue)
            {
                return BadRequest(new
                {
                    message = "SkillID required."
                });
            }

            UserSkillDto? userSkill = await _userSkillService.AddSelfReportedAsync(userId, request.SkillID.Value, cancellationToken);

            if (userSkill is null)
            {
                return BadRequest(new
                {
                    message = "User or skill cannot be found."
                });
            }

            return StatusCode(StatusCodes.Status201Created, userSkill);
        }

        [HttpDelete("{skillId:guid}")]
        public async Task<ActionResult> RemoveSkill(Guid skillId, CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }

            bool removed = await _userSkillService.RemoveSelfReportedAsync(userId, skillId, cancellationToken);

            if (!removed)
            {
                return NotFound(new
                {
                    message = "User skill cannot be found."
                });
            }

            return NoContent();
        }

        private bool TryGetCurrentUserId(out Guid userId)
        {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userIdClaim, out userId);
        }
    }
}
