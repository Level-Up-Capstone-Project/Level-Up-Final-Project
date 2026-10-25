using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.SavedResources;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Services
{
    /// <summary>
    /// Coordinates Saved Resource service application behavior.
    /// </summary>
    public class SavedResourceService : ISavedResourceService
    {
        private readonly AppDbContext _context;

        public SavedResourceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<SavedResourceDto>> GetAllAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return await _context.SavedResources
                .AsNoTracking()
                .Where(saved =>
                    saved.UserID == userId &&
                    saved.Resource.IsActive)
                .OrderByDescending(saved => saved.SavedAt)
                .Select(saved => new SavedResourceDto
                {
                    SavedResourceID = saved.SavedResourceID,
                    ResourceID = saved.ResourceID,
                    ResourceName = saved.Resource.ResourceName,
                    ResourceType = saved.Resource.ResourceType,
                    Url = saved.Resource.Url,
                    Provider = saved.Resource.Provider,
                    DifficultyLevel = saved.Resource.DifficultyLevel,
                    CostType = saved.Resource.CostType,
                    EstDuration = saved.Resource.EstDuration,
                    Description = saved.Resource.Description,
                    SavedAt = saved.SavedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<SavedResourceDto> SaveAsync(
            Guid userId,
            Guid resourceId,
            CancellationToken cancellationToken)
        {
            bool userExists = await _context.Users
                .AsNoTracking()
                .AnyAsync(user => user.UserID == userId, cancellationToken);

            if (!userExists)
            {
                throw new KeyNotFoundException("The user could not be found.");
            }

            LearningResource? resource = await _context.LearningResources
                .AsNoTracking()
                .SingleOrDefaultAsync(item =>
                    item.ResourceID == resourceId && item.IsActive,
                    cancellationToken);

            if (resource is null)
            {
                throw new KeyNotFoundException("The active learning resource could not be found.");
            }

            await using var transaction = await _context.Database
                .BeginTransactionAsync(cancellationToken);

            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT pg_advisory_xact_lock(hashtext({userId.ToString()}), hashtext({resourceId.ToString()}))",
                cancellationToken);

            SavedResource? saved = await _context.SavedResources
                .Where(item => item.UserID == userId && item.ResourceID == resourceId)
                .OrderBy(item => item.SavedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (saved is null)
            {
                saved = new SavedResource
                {
                    SavedResourceID = Guid.NewGuid(),
                    UserID = userId,
                    ResourceID = resourceId,
                    SavedAt = DateTime.UtcNow
                };

                _context.SavedResources.Add(saved);
                await _context.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);

            return Map(saved, resource);
        }

        public async Task<bool> DeleteAsync(
            Guid userId,
            Guid resourceId,
            CancellationToken cancellationToken)
        {
            int deleted = await _context.SavedResources
                .Where(saved => saved.UserID == userId && saved.ResourceID == resourceId)
                .ExecuteDeleteAsync(cancellationToken);

            return deleted > 0;
        }

        private static SavedResourceDto Map(
            SavedResource saved,
            LearningResource resource)
        {
            return new SavedResourceDto
            {
                SavedResourceID = saved.SavedResourceID,
                ResourceID = resource.ResourceID,
                ResourceName = resource.ResourceName,
                ResourceType = resource.ResourceType,
                Url = resource.Url,
                Provider = resource.Provider,
                DifficultyLevel = resource.DifficultyLevel,
                CostType = resource.CostType,
                EstDuration = resource.EstDuration,
                Description = resource.Description,
                SavedAt = saved.SavedAt
            };
        }
    }
}
