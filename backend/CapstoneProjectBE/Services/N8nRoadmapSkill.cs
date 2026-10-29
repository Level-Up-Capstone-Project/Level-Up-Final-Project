namespace CapstoneProjectBE.Services;

/// <summary>
/// Identifies an existing database resource so the roadmap workflow does not recommend it again.
/// </summary>
public sealed record N8nExistingLearningResource(
    Guid ResourceID,
    string ResourceName,
    string Provider,
    string Url,
    string? ProviderResourceID);

/// <summary>
/// Stores immutable N8n Roadmap Skill values used by application services.
/// </summary>
public sealed record N8nRoadmapSkill(
    Guid SkillID,
    string SkillName,
    string? Category,
    int? ImportanceLevel,
    bool IsCoreSkill,
    string Status,
    int ResourceSlots = 0,
    IReadOnlyList<N8nExistingLearningResource>? ExistingResources = null);
