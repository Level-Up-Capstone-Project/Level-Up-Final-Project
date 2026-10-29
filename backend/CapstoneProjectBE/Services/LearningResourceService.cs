using CapstoneProjectBE.Services.Interfaces;
using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.Resources;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Services
{
    //read operations for learning resources (external)
    //inactive resources are soft-deleted

    /// <summary>
    /// Coordinates Learning Resource service application behavior.
    /// </summary>
    public class LearningResourceService : ILearningResourceService
    {
        private readonly AppDbContext _context;

        public LearningResourceService (AppDbContext context)
        {
            _context = context;

        }

        //GET /api/resurces/{id}
        //returns null for missing or inactive resources
        public async Task<LearningResourceDto?> GetByIdAsync(Guid resourceId, CancellationToken cancellationToken)
        {
            return await _context.LearningResources.AsNoTracking()
            .Where (resource => resource.ResourceID == resourceId && resource.IsActive)
            .Select(resource => new LearningResourceDto
            {
                ResourceID = resource.ResourceID,
                SkillID = resource.SkillID,
                ResourceName = resource.ResourceName,
                ResourceType = resource.ResourceType,
                Provider = resource.Provider,
                ProviderResourceID = resource.ProviderResourceID,
                Url = resource.Url,
                DifficultyLevel = resource.DifficultyLevel,
                CostType = resource.CostType,
                EstDuration = resource.EstDuration,
                Description = resource.Description

            })
            .SingleOrDefaultAsync(cancellationToken);
        }

        //GET api/resources?skillID=
        //returns empty list (not null) when a skill has no resources
        public async Task<IReadOnlyList<LearningResourceDto>> GetBySkillAsync (Guid skillId, CancellationToken cancellationToken)
        {
            return await _context.LearningResources.AsNoTracking()
            .Where (resource => resource.SkillID == skillId && resource .IsActive)
            .OrderBy(resource => resource.ResourceName)
            .Select(resource => new LearningResourceDto
            {
                ResourceID = resource.ResourceID,
                    SkillID = resource.SkillID,
                    ResourceName = resource.ResourceName,
                    ResourceType = resource.ResourceType,
                    Url = resource.Url,
                    Provider = resource.Provider,
                    ProviderResourceID = resource.ProviderResourceID,
                    DifficultyLevel = resource.DifficultyLevel,
                    CostType = resource.CostType,
                    EstDuration = resource.EstDuration,
                    Description = resource.Description
            })
            .ToListAsync(cancellationToken);
        }

    }
    
}
