using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CapstoneProjectBE.DTOs.Careers;

namespace CapstoneProjectBE.Controllers
{
    /// <summary>
    /// Handles HTTP requests for Careers operations.
    /// </summary>
    [ApiController]
    [Route("api/careers")]
    public class CareersController : ControllerBase
    {
        private readonly ICareerService _careerService;

        public CareersController(ICareerService careerService)
        {
            _careerService = careerService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CareerDto>>> GetAll(CancellationToken cancellationToken)
        {
            var careers = await _careerService.GetAllAsync(cancellationToken);
            return Ok(careers);
        }

        [HttpGet("{careerId:guid}")]
        public async Task<ActionResult<CareerDto>> GetById( Guid careerId, CancellationToken cancellationToken)
        {
            var career = await _careerService.GetByIdAsync(careerId, cancellationToken);

            if (career is null)
            {
                return NotFound();
            }

            return Ok(career);
        }

        [HttpGet("{careerId:guid}/skills")]
        public async Task<ActionResult<IReadOnlyList<CareerSkillDto>>> GetSkill(Guid careerId, CancellationToken cancellationToken)
        {
            IReadOnlyList<CareerSkillDto>? skills = await _careerService.GetSkillsAsync(careerId, cancellationToken);

            if(skills is null)
            {
                return NotFound(new
                {
                    message = "Career cannot be found."
                });
            }
            return Ok(skills);
        }
    }
}
