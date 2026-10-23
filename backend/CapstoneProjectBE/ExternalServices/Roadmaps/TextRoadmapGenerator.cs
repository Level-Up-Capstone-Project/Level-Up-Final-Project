using CapstoneProjectBE.Services;

namespace CapstoneProjectBE.ExternalServices.Roadmaps;

/// <summary>Builds a safe roadmap from trusted catalogue skills when the AI workflow cannot.</summary>
public sealed class TextRoadmapGenerator
{
    public GeneratedRoadmap Generate(string careerName, IReadOnlyList<N8nRoadmapSkill> skills)
    {
        var actionable = skills.Where(skill => skill.Status is "Missing" or "Partial")
            .OrderByDescending(skill => skill.IsCoreSkill)
            .ThenByDescending(skill => skill.ImportanceLevel ?? 0)
            .ThenBy(skill => skill.SkillName)
            .ToList();
        var items = actionable.Select(skill => new GeneratedRoadmapItem(
            skill.SkillID,
            Limit(skill.Status == "Missing" ? $"Learn {skill.SkillName}" : $"Strengthen {skill.SkillName}", 150),
            Limit(skill.Status == "Missing"
                ? $"Build a foundation in {skill.SkillName} by completing the recommended learning resources and applying the skill in a practical exercise."
                : $"Deepen your {skill.SkillName} capability through focused practice and the recommended learning resources.", 2000),
            Priority(skill))).ToList();
        return new GeneratedRoadmap(Limit($"{careerName} Learning Roadmap", 150), items);
    }

    private static string Priority(N8nRoadmapSkill skill) =>
        skill.IsCoreSkill || skill.ImportanceLevel >= 4 ? "high" : skill.ImportanceLevel >= 2 ? "medium" : "low";

    private static string Limit(string value, int max) => value.Length <= max ? value : value[..max].TrimEnd();
}
