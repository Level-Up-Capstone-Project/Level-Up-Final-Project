using System.Security.Claims;
using CapstoneProjectBE.DTOs.SavedResources;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProjectBE.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/saved-resources")]
    public class SavedResourcesController : ControllerBase
    {
        private readonly ISavedResourceService _savedResourceService;

        public SavedResourcesController(ISavedResourceService savedResourceService)
        {
            _savedResourceService = savedResourceService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<SavedResourceDto>>> GetAll(
            CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }

            IReadOnlyList<SavedResourceDto> resources = await _savedResourceService
                .GetAllAsync(userId, cancellationToken);

            return Ok(resources);
        }

        [HttpPost("{resourceId:guid}")]
        public async Task<ActionResult<SavedResourceDto>> Save(
            Guid resourceId,
            CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }

            try
            {
                SavedResourceDto resource = await _savedResourceService
                    .SaveAsync(userId, resourceId, cancellationToken);

                return Ok(resource);
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(new { message = exception.Message });
            }
        }

        [HttpDelete("{resourceId:guid}")]
        public async Task<IActionResult> Delete(
            Guid resourceId,
            CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out Guid userId))
            {
                return Unauthorized();
            }

            bool deleted = await _savedResourceService
                .DeleteAsync(userId, resourceId, cancellationToken);

            return deleted ? NoContent() : NotFound();
        }

        private bool TryGetCurrentUserId(out Guid userId)
        {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out userId);
        }
    }
}
