using System.Text.Json;
using CapstoneProjectBE.Services;

namespace CapstoneProjectBE.ExternalServices.Roadmaps;

/// <summary>
/// Carries a validated learning-resource candidate returned for a weak roadmap skill.
/// </summary>
public sealed record GeneratedLearningResource(
    string ResourceName,
    string ResourceType,
    string CanonicalUrl,
    string Provider,
    string ProviderKey,
    string? ProviderResourceID,
    string DifficultyLevel,
    string CostType,
    string EstDuration,
    string Description);

/// <summary>
/// Carries a generated roadmap step and any validated resources needed to fill its catalogue gaps.
/// </summary>
public sealed record GeneratedRoadmapItem(
    Guid SkillID,
    string Title,
    string Description,
    string Priority,
    IReadOnlyList<GeneratedLearningResource> Resources);
/// <summary>
/// Carries Generated Roadmap values used by an external integration.
/// </summary>
public sealed record GeneratedRoadmap(string Title, IReadOnlyList<GeneratedRoadmapItem> Items);

/// <summary>
/// Provides N8n Roadmap Generator integration behavior.
/// </summary>
public sealed class N8nRoadmapGenerator(
    N8nAnalysisService workflow,
    TextRoadmapGenerator fallback,
    ILogger<N8nRoadmapGenerator>? logger = null)
{
    public async Task<GeneratedRoadmap> GenerateAsync(
        Guid careerId,
        string careerName,
        IReadOnlyList<N8nRoadmapSkill> skills,
        CancellationToken cancellationToken)
    {
        var requestId = Guid.NewGuid();
        try
        {
            var result = await workflow.GenerateRoadmapAsync(requestId, careerId, careerName, skills, cancellationToken);
            return Validate(result, requestId, careerId, skills);
        }
        catch (N8nAnalysisException error) when (!cancellationToken.IsCancellationRequested)
        {
            logger?.LogWarning(
                "Roadmap n8n generation failed with backend status {StatusCode} and upstream status {UpstreamStatusCode}; using deterministic fallback.",
                error.StatusCode, error.UpstreamStatusCode);
            return fallback.Generate(careerName, skills);
        }
    }

    public static GeneratedRoadmap Validate(
        JsonElement root,
        Guid requestId,
        Guid careerId,
        IReadOnlyList<N8nRoadmapSkill> requestedSkills)
    {
        static N8nAnalysisException Invalid() => new("The roadmap response did not match the expected format.");
        static bool TryProperty(JsonElement item, string name, out JsonElement value) =>
            N8nResponseParser.TryGetProperty(item, name, out value);

        static JsonElement Unwrap(JsonElement value)
        {
            try { return N8nResponseParser.Unwrap(value); }
            catch (N8nAnalysisException) { throw Invalid(); }
        }

        static string Text(JsonElement item, string name, int max)
        {
            if (!TryProperty(item, name, out var value) || value.ValueKind != JsonValueKind.String) throw Invalid();
            var text = value.GetString()?.Trim();
            if (string.IsNullOrWhiteSpace(text) || text.Length > max) throw Invalid();
            return text;
        }

        static string? OptionalText(JsonElement item, string name, int max)
        {
            if (!TryProperty(item, name, out var value) || value.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            if (value.ValueKind != JsonValueKind.String) throw Invalid();
            var text = value.GetString()?.Trim();
            if (string.IsNullOrWhiteSpace(text) || text.Length > max) throw Invalid();
            return text;
        }

        static string Choice(JsonElement item, string name, params string[] allowed)
        {
            string value = Text(item, name, 50);
            return allowed.SingleOrDefault(option =>
                       string.Equals(option, value, StringComparison.OrdinalIgnoreCase))
                   ?? throw Invalid();
        }

        static IReadOnlyList<GeneratedLearningResource> Resources(
            JsonElement item,
            N8nRoadmapSkill skill,
            HashSet<string> identities)
        {
            if (!TryProperty(item, "resources", out var values))
            {
                return Array.Empty<GeneratedLearningResource>();
            }

            if (values.ValueKind != JsonValueKind.Array ||
                values.GetArrayLength() > skill.ResourceSlots ||
                values.GetArrayLength() > 3)
            {
                throw Invalid();
            }

            var resources = new List<GeneratedLearningResource>();
            foreach (JsonElement value in values.EnumerateArray())
            {
                if (value.ValueKind != JsonValueKind.Object) throw Invalid();
                string provider = Text(value, "provider", 100);
                string providerKey = LearningResourceIdentity.ProviderKey(provider);
                string? providerResourceId = OptionalText(value, "providerResourceID", 200);
                string url = Text(value, "url", 500);
                if (providerKey.Length == 0 ||
                    !LearningResourceIdentity.TryCanonicalizeHttpsUrl(url, out string canonicalUrl))
                {
                    throw Invalid();
                }

                if (!identities.Add($"{skill.SkillID}:url:{canonicalUrl}") ||
                    providerResourceId is not null &&
                    !identities.Add($"{skill.SkillID}:provider:{providerKey}:{providerResourceId}"))
                {
                    throw Invalid();
                }

                resources.Add(new GeneratedLearningResource(
                    Text(value, "resourceName", 200),
                    Choice(value, "resourceType", "Course", "Learning Path", "Tutorial", "Guide", "Practice", "Project", "Video", "Book", "Documentation", "Article"),
                    canonicalUrl,
                    provider,
                    providerKey,
                    providerResourceId,
                    Choice(value, "difficultyLevel", "Beginner", "Intermediate", "Advanced", "All Levels"),
                    Choice(value, "costType", "Free", "Freemium", "Paid"),
                    Text(value, "estDuration", 100),
                    Text(value, "description", 2000)));
            }

            return resources;
        }

        root = Unwrap(root);
        if (root.ValueKind != JsonValueKind.Object ||
            !TryProperty(root, "success", out var success) || success.ValueKind != JsonValueKind.True ||
            !Guid.TryParse(Text(root, "requestId", 36), out var returnedRequestId) || returnedRequestId != requestId ||
            !Guid.TryParse(Text(root, "careerID", 36), out var returnedCareerId) || returnedCareerId != careerId ||
            !TryProperty(root, "roadmap", out var roadmap) || roadmap.ValueKind != JsonValueKind.Object ||
            !TryProperty(roadmap, "items", out var items) || items.ValueKind != JsonValueKind.Array)
            throw Invalid();

        var requiredSkills = requestedSkills.Where(skill => skill.Status is "Missing" or "Partial")
            .Select(skill => skill.SkillID).ToHashSet();
        var requestedById = requestedSkills.ToDictionary(skill => skill.SkillID);
        if (items.GetArrayLength() != requiredSkills.Count || items.GetArrayLength() > 100) throw Invalid();

        var generated = new List<GeneratedRoadmapItem>();
        var generatedTitles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var resourceIdentities = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in items.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object ||
                !Guid.TryParse(Text(item, "skillID", 36), out var skillId) || !requiredSkills.Remove(skillId))
                throw Invalid();
            N8nRoadmapSkill requestedSkill = requestedById[skillId];
            var priority = Text(item, "priority", 10).ToLowerInvariant();
            if (priority is not ("high" or "medium" or "low")) throw Invalid();
            var title = Text(item, "title", 150);
            if (!generatedTitles.Add(title)) throw Invalid();
            generated.Add(new GeneratedRoadmapItem(
                skillId,
                title,
                Text(item, "description", 2000),
                priority,
                Resources(item, requestedSkill, resourceIdentities)));
        }
        if (requiredSkills.Count != 0) throw Invalid();
        return new GeneratedRoadmap(Text(roadmap, "title", 150), generated);
    }
}
