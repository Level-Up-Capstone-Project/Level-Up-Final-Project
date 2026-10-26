using System.Text.Json;
using CapstoneProjectBE.Services;

namespace CapstoneProjectBE.ExternalServices.Roadmaps;

/// <summary>
/// Carries Generated Roadmap Item values used by an external integration.
/// </summary>
public sealed record GeneratedRoadmapItem(Guid SkillID, string Title, string Description, string Priority);
/// <summary>
/// Carries Generated Roadmap values used by an external integration.
/// </summary>
public sealed record GeneratedLearningResource(string Provider, string CourseId, string Title, string Url, IReadOnlyList<string> MatchedSkills, string AccessType);
public sealed record GeneratedRoadmap(string Title, IReadOnlyList<GeneratedRoadmapItem> Items, IReadOnlyList<GeneratedLearningResource>? LearningResources = null);

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
        if (items.GetArrayLength() != requiredSkills.Count || items.GetArrayLength() > 100) throw Invalid();

        var generated = new List<GeneratedRoadmapItem>();
        var generatedTitles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in items.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object ||
                !Guid.TryParse(Text(item, "skillID", 36), out var skillId) || !requiredSkills.Remove(skillId))
                throw Invalid();
            var priority = Text(item, "priority", 10).ToLowerInvariant();
            if (priority is not ("high" or "medium" or "low")) throw Invalid();
            var title = Text(item, "title", 150);
            if (!generatedTitles.Add(title)) throw Invalid();
            generated.Add(new GeneratedRoadmapItem(skillId, title, Text(item, "description", 2000), priority));
        }
        if (requiredSkills.Count != 0) throw Invalid();
        var resources = new List<GeneratedLearningResource>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (TryProperty(roadmap, "learningResources", out var recommendations) || TryProperty(root, "learningResources", out recommendations))
        {
            if (recommendations.ValueKind != JsonValueKind.Array || recommendations.GetArrayLength() > 100) throw Invalid();
            foreach (var resource in recommendations.EnumerateArray())
            {
                if (resource.ValueKind != JsonValueKind.Object) throw Invalid();
                var provider = Text(resource, "provider", 100);
                var courseId = Text(resource, "courseId", 200);
                var title = Text(resource, "title", 200);
                var url = Text(resource, "url", 500);
                var accessType = Text(resource, "accessType", 100);
                if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps ||
                    !string.IsNullOrEmpty(uri.UserInfo)) throw Invalid();
                if (!TryProperty(resource, "matchedSkills", out var matches) || matches.ValueKind != JsonValueKind.Array || matches.GetArrayLength() > 100) throw Invalid();
                var names = new List<string>();
                foreach (var match in matches.EnumerateArray())
                {
                    if (match.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(match.GetString()) || match.GetString()!.Length > 200) throw Invalid();
                    names.Add(match.GetString()!.Trim());
                }
                if (seen.Add(provider + "\n" + courseId)) resources.Add(new(provider, courseId, title, url, names, accessType));
            }
        }
        return new GeneratedRoadmap(Text(roadmap, "title", 150), generated, resources);
    }
}
