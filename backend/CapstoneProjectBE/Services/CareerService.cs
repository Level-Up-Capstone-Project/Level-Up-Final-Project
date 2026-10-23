using CapstoneProjectBE.Services.Interfaces;
using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.Careers;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Services
{
    public class CareerService : ICareerService
    {
        private readonly AppDbContext _context;

        public CareerService(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<IReadOnlyList<CareerDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            //AsNoTracking useful for read-only queries
            return await _context.Careers.AsNoTracking().OrderBy(career => career.CareerName).Select(career => new CareerDto
            {
                CareerID = career.CareerID,
                CareerName = career.CareerName,
                Description = career.Description,
                Industry = career.Industry,
                EducationRequirements = career.EducationRequirements
            }).ToListAsync(cancellationToken);
        }

        public async Task<CareerDto?> GetByIdAsync( Guid careerId, CancellationToken cancellationToken)
        {
            return await _context.Careers.AsNoTracking().Where(career => career.CareerID == careerId).Select(career => new CareerDto
            {
                CareerID = career.CareerID,
                CareerName = career.CareerName,
                Description = career.Description,
                Industry = career.Industry,
                EducationRequirements = career.EducationRequirements
            }).SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<CareerSkillDto>?> GetSkillsAsync(Guid careerId, CancellationToken cancellationToken)
        {
            bool careerExists = await _context.Careers.AnyAsync(career => career.CareerID == careerId, cancellationToken);

            if(!careerExists)
                return null;

            return await _context.CareerSkills.AsNoTracking().Where(careerSkill => careerSkill.CareerID == careerId).OrderByDescending(careerSkill => careerSkill.IsCoreSkill).ThenByDescending(careerSkill => careerSkill.ImportanceLevel).Select(careerSkill => new CareerSkillDto
            {
                SkillID = careerSkill.SkillID,
                SkillName = careerSkill.Skill.SkillName,
                Category = careerSkill.Skill.Category,
                ImportanceLevel = careerSkill.ImportanceLevel,
                IsCoreSkill = careerSkill.IsCoreSkill
            }).ToListAsync(cancellationToken);
        }
    }
}
