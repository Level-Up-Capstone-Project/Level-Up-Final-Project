using System.Security.Claims;
using CapstoneProjectBE.DTOs.CareerReadiness;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProjectBE.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/career-readiness")]
    public class CareerReadinessController : ControllerBase
    {
        private readonly ICareerReadinessService _careerReadinessService;

        public CareerReadinessController(ICareerReadinessService careerReadinessService)
        {
            _careerReadinessService = careerReadinessService;
        }

        [HttpGet]
        public async Task<ActionResult<CareerReadinessDto>> GetCareerReadiness(CancellationToken cancellationToken)
        {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized();
            }

            CareerReadinessDto? readiness = await _careerReadinessService.GetCareerReadinessAsync(userId, cancellationToken);

            if (readiness is null)
            {
                return NotFound(new
                {
                    message = "A target career and analysed CV are required."
                });
            }
            return Ok(readiness);
        }
    }
}
