using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.Skills;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Services
{
    public class SkillService : ISkillService
    {
        private readonly AppDbContext _context;

        public SkillService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<SkillDto>> GetAllAsync(string? category, CancellationToken cancellationToken)
        {
            IQueryable<Skill> query = _context.Skills.AsNoTracking();

            if(!string.IsNullOrWhiteSpace(category))
            {
                string normalizeCategory = category.Trim();

                query = query.Where(skill => EF.Functions.ILike(skill.Category, normalizeCategory));
            }

            return await query.OrderBy(skill => skill.SkillName).Select(skill => new SkillDto
            {
                SkillID = skill.SkillID,
                SkillName = skill.SkillName,
                Category = skill.Category,
                Description = skill.Description
            }).ToListAsync(cancellationToken);
        }

        public async Task<SkillDto?> GetByIdAsync(Guid skillId, CancellationToken cancellationToken)
        {
            return await _context.Skills.AsNoTracking().Where(skill => skill.SkillID == skillId).Select(skill => new SkillDto
            {
                SkillID = skill.SkillID,
                SkillName = skill.SkillName,
                Category = skill.Category,
                Description = skill.Description
            }).SingleOrDefaultAsync(cancellationToken);
        }
    }
}
