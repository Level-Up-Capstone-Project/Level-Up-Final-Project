using System.Text.Json;
using CapstoneProjectBE.Services;

namespace CapstoneProjectBE.ExternalServices.CVAnalysis;

/// <summary>
/// Provides N8n CVAnalysis Provider integration behavior.
/// </summary>
public sealed class N8nCVAnalysisProvider(
    N8nAnalysisService workflow,
    ICVTextExtractor extractor,
    TextCVAnalysisProvider fallback,
    ILogger<N8nCVAnalysisProvider>? logger = null) : ICVAnalysisProvider
{
    // Configuration is checked when n8n is called so a missing workflow can use the local fallback.
    public void EnsureConfigured() { }

    public async Task<CVAnalysisResult> AnalyseAsync(CVAnalysisRequest request, CancellationToken ct)
    {
        var safeFileName = PersonalInformationRedactor.RedactFileName(request.FileName);
        var text = await extractor.ExtractTextAsync(request.Document, request.ContentType, ct);
        if (text.Count(char.IsLetterOrDigit) < 50)
            throw new InvalidDataException("The CV does not contain enough readable text.");
        DocumentOwnershipVerifier.EnsureMatches(request.Owner, text, "CV");
        try
        {
            var json = await workflow.AnalyseCvAsync(request.RequestId, request.CVID, request.CareerID, request.CareerName,
                safeFileName, PersonalInformationRedactor.Redact(text), ct);
            var result = Validate(json, request);
            result.DetectedSkills = result.DetectedSkills.Concat(fallback.DetectSkills(text, request.KnownSkills))
                .DistinctBy(skill => skill.SkillID).OrderBy(skill => skill.SkillName).ToList();
            return result;
        }
        catch (N8nAnalysisException error) when (!ct.IsCancellationRequested)
        {
            logger?.LogWarning(
                "CV n8n analysis failed with backend status {StatusCode} and upstream status {UpstreamStatusCode}; using text fallback.",
                error.StatusCode, error.UpstreamStatusCode);
            return fallback.Analyse(text, request.KnownSkills);
        }
    }

    public static CVAnalysisResult Validate(JsonElement root, CVAnalysisRequest request)
    {
        static N8nAnalysisException Invalid() => new("The CV analysis response did not match the expected format.");
        static bool TryProperty(JsonElement item, string field, out JsonElement value) =>
            N8nResponseParser.TryGetProperty(item, field, out value);

        static JsonElement Unwrap(JsonElement value)
        {
            try { return N8nResponseParser.Unwrap(value); }
            catch (N8nAnalysisException) { throw Invalid(); }
        }

        static string Text(JsonElement item, string field, int max)
        {
            if (!TryProperty(item, field, out var value) || value.ValueKind != JsonValueKind.String ||
                string.IsNullOrWhiteSpace(value.GetString()) || value.GetString()!.Length > max) throw Invalid();
            return value.GetString()!.Trim();
        }
        static decimal Score(JsonElement item, string field)
        {
            if (!TryProperty(item, field, out var value) || value.ValueKind != JsonValueKind.Number ||
                !value.TryGetDecimal(out var score) || score < 0 || score > 100) throw Invalid();
            return score;
        }

        root = Unwrap(root);
        if (root.ValueKind != JsonValueKind.Object || !TryProperty(root, "success", out var success) || success.ValueKind != JsonValueKind.True ||
            !Text(root, "analysisType", 40).Equals("cv", StringComparison.OrdinalIgnoreCase) ||
            !TryProperty(root, "analysis", out var analysis) || analysis.ValueKind != JsonValueKind.Object) throw Invalid();
        foreach (var pair in new[] {("requestId", request.RequestId), ("cvID", request.CVID)})
            if (TryProperty(root, pair.Item1, out var id) && (id.ValueKind != JsonValueKind.String || !id.TryGetGuid(out var value) || value != pair.Item2)) throw Invalid();
        if (!TryProperty(analysis, "detectedSkills", out var array) || array.ValueKind != JsonValueKind.Array || array.GetArrayLength() > 100) throw Invalid();
        var skills = new List<CVSkillCandidate>();
        foreach (var item in array.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object) throw Invalid();
            var name = Text(item, "skillName", 100);
            Text(item, "evidence", 3000);
            // The workflow cannot supply database skill IDs or mark a skill knowledge-verified.
            var known = request.KnownSkills.SingleOrDefault(skill => SkillNameMatcher.NamesMatch(skill.SkillName, name));
            if (known is not null && !skills.Any(skill => skill.SkillID == known.SkillID)) skills.Add(known);
        }
        return new CVAnalysisResult
        {
            QualityScore = Score(analysis, "qualityScore"), ATSScore = Score(analysis, "atsScore"), FormattingScore = Score(analysis, "formattingScore"),
            Strengths = Text(analysis, "strengths", 10000), Weaknesses = Text(analysis, "weaknesses", 10000),
            Recommendations = Text(analysis, "recommendations", 10000), DetectedSkills = skills,
        };
    }
}
