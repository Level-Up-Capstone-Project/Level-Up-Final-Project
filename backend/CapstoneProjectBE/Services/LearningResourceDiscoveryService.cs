using CapstoneProjectBE.Data;
using CapstoneProjectBE.ExternalServices.Roadmaps;
using CapstoneProjectBE.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Services;

/// <summary>
/// Persists validated AI-discovered learning resources and reuses matching database records across roadmaps.
/// </summary>
public sealed class LearningResourceDiscoveryService(AppDbContext context)
{
    public async Task PersistAsync(GeneratedRoadmap roadmap, CancellationToken cancellationToken)
    {
        foreach (IGrouping<Guid, GeneratedLearningResource> skillResources in roadmap.Items
                     .SelectMany(item => item.Resources.Select(resource => new { item.SkillID, Resource = resource }))
                     .GroupBy(item => item.SkillID, item => item.Resource)
                     .OrderBy(group => group.Key))
        {
            await PersistSkillResourcesAsync(skillResources.Key, skillResources, cancellationToken);
        }
    }

    private async Task PersistSkillResourcesAsync(
        Guid skillId,
        IEnumerable<GeneratedLearningResource> candidates,
        CancellationToken cancellationToken)
    {
        await using var transaction = context.Database.IsRelational()
            ? await context.Database.BeginTransactionAsync(cancellationToken)
            : null;

        if (context.Database.IsNpgsql())
        {
            const string lockScope = "learning-resource";
            await context.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT pg_advisory_xact_lock(hashtext({lockScope}), hashtext({skillId.ToString()}))",
                cancellationToken);
        }

        List<LearningResource> existing = await context.LearningResources
            .Where(resource => resource.SkillID == skillId)
            .ToListAsync(cancellationToken);

        foreach (GeneratedLearningResource candidate in candidates)
        {
            LearningResource? providerMatch = existing.FirstOrDefault(item =>
                candidate.ProviderResourceID is not null &&
                item.ProviderKey == candidate.ProviderKey &&
                item.ProviderResourceID == candidate.ProviderResourceID);
            LearningResource? urlMatch = existing.FirstOrDefault(item =>
                item.CanonicalUrl == candidate.CanonicalUrl);
            if (providerMatch is not null && urlMatch is not null && providerMatch != urlMatch)
            {
                continue;
            }

            LearningResource? resource = providerMatch ?? urlMatch;

            if (resource is null)
            {
                resource = new LearningResource
                {
                    ResourceID = Guid.NewGuid(),
                    SkillID = skillId,
                    ResourceName = candidate.ResourceName,
                    ResourceType = candidate.ResourceType,
                    Url = candidate.CanonicalUrl,
                    CanonicalUrl = candidate.CanonicalUrl,
                    Provider = candidate.Provider,
                    ProviderKey = candidate.ProviderKey,
                    ProviderResourceID = candidate.ProviderResourceID,
                    DifficultyLevel = candidate.DifficultyLevel,
                    CostType = candidate.CostType,
                    EstDuration = candidate.EstDuration,
                    Description = candidate.Description,
                    SourceType = "AI",
                    IsActive = true
                };

                context.LearningResources.Add(resource);
                existing.Add(resource);
                continue;
            }

            if (!string.Equals(resource.SourceType, "AI", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            resource.ResourceName = candidate.ResourceName;
            resource.ResourceType = candidate.ResourceType;
            resource.Url = candidate.CanonicalUrl;
            resource.CanonicalUrl = candidate.CanonicalUrl;
            resource.Provider = candidate.Provider;
            resource.ProviderKey = candidate.ProviderKey;
            resource.ProviderResourceID = candidate.ProviderResourceID;
            resource.DifficultyLevel = candidate.DifficultyLevel;
            resource.CostType = candidate.CostType;
            resource.EstDuration = candidate.EstDuration;
            resource.Description = candidate.Description;
        }

        await context.SaveChangesAsync(cancellationToken);
        if (transaction is not null)
        {
            await transaction.CommitAsync(cancellationToken);
        }
    }
}
