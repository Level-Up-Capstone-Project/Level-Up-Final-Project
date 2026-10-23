using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CapstoneProjectBE.DTOs.Profile;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CapstoneProjectBE.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<ActionResult<ProfileDto>> GetProfile(CancellationToken cancellationToken)
        {
            if(!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }
             ProfileDto? profile = await _profileService.GetProfileAsync(userId, cancellationToken);

            if(profile is null)
            {
                return NotFound(new
                {
                    message = "Profile has not been created."
                });
            }
            return Ok(profile);
        }

        [HttpPut]
        public async Task<ActionResult<ProfileDto>> UpdateProfile([FromBody] UpdateProfileDto request, CancellationToken cancellationToken)
        {
            if(!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }

            ProfileDto? profile = await _profileService.UpsertProfileAsync(userId, request, cancellationToken);

            if(profile is null)
            {
                return BadRequest(new
                {
                    message = "Unable to save profile."
                });
            }
            return Ok(profile);
        }

        [HttpPut("preferences")]
        public async Task<ActionResult<ProfileDto>> UpdatePreferences([FromBody] UpdateProfilePreferencesDto request, CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out Guid userId)) return Unauthorized();
            if (request.TargetRoles?.Any(role => string.IsNullOrWhiteSpace(role) || role.Trim().Length > 100) == true)
                return BadRequest(new {message = "Target roles must be no longer than 100 characters."});

            ProfileDto? profile = await _profileService.UpdatePreferencesAsync(userId, request, cancellationToken);
            return profile is null ? NotFound() : Ok(profile);
        }

        [HttpPut("target-career")]
        public async Task<ActionResult<ProfileDto>> UpdateTargetCareer([FromBody] UpdateTargetCareerDto request, CancellationToken cancellationToken)
        {
            if(!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }

            if(!request.CareerID.HasValue)
            {
                return BadRequest(new
                {
                    message = "CareerID is required."
                });
            }

            ProfileDto? profile = await _profileService.UpdateTargetCareerAsync(userId, request.CareerID.Value, cancellationToken);
             if( profile is null)
            {
                return BadRequest(new
                {
                    message = "Profile or Career could not be found."
                });
            }
             return Ok(profile);
        }

        private bool TryGetCurrentUserId(out Guid userId)
        {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out userId);
        }
    }
}
