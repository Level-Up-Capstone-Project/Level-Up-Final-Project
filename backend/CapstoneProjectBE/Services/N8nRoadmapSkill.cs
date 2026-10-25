namespace CapstoneProjectBE.Services;

/// <summary>
/// Stores immutable N8n Roadmap Skill values used by application services.
/// </summary>
public sealed record N8nRoadmapSkill(Guid SkillID, string SkillName, string? Category, int? ImportanceLevel, bool IsCoreSkill, string Status);
