using CapstoneProjectBE.Services;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Globalization;
using CapstoneProjectBE.ExternalServices.CVAnalysis;

namespace CapstoneProjectBE.ExternalServices.Transcripts;

/// <summary>Calls a fixed, server-configured webhook. No URLs or auth headers come from uploaded content.</summary>
public sealed class N8nTranscriptAnalysisProvider(CapstoneProjectBE.Services.N8nAnalysisService workflow,
    [FromKeyedServices("Transcript")] CapstoneProjectBE.ExternalServices.CVAnalysis.ICVTextExtractor extractor) : ITranscriptAnalysisProvider
{
    public void EnsureConfigured() => workflow.EnsureConfigured("Transcript");

    public async Task<TranscriptInsights> AnalyseAsync(string? careerName, string fileName, string contentType,
        byte[] document, CancellationToken ct, IReadOnlyList<string>? knownSkills = null)
    {
        knownSkills ??= [];
        var safeFileName = PersonalInformationRedactor.RedactFileName(fileName);
        var text = await extractor.ExtractTextAsync(document, contentType, ct);
        if (text.Count(char.IsLetterOrDigit) < 50)
            throw new InvalidDataException("The transcript does not contain enough readable text.");
        var json = await workflow.AnalyseTranscriptAsync(careerName, safeFileName, PersonalInformationRedactor.Redact(text), ct);
        var result = Validate(json);
        var localSkills = knownSkills.Where(skill => SkillNameMatcher.TextContains(text, skill))
            .Select(skill => new TranscriptSkillEvidence(skill, $"{skill} appears in the extracted transcript text."));
        var detectedSkills = result.DetectedSkills.ToList();
        foreach (var localSkill in localSkills)
            if (!detectedSkills.Any(skill => SkillNameMatcher.NamesMatch(skill.SkillName, localSkill.SkillName)))
                detectedSkills.Add(localSkill);
        return result with
        {
            DetectedSkills = detectedSkills
        };
    }

    public static TranscriptInsights Validate(JsonElement root)
    {
        static N8nAnalysisException Invalid() => new("The transcript analysis response did not match the expected format.");
        static bool TryProperty(JsonElement item, string field, out JsonElement value) =>
            N8nResponseParser.TryGetProperty(item, field, out value);

        static JsonElement Unwrap(JsonElement value)
        {
            try { return N8nResponseParser.Unwrap(value); }
            catch (N8nAnalysisException) { throw Invalid(); }
        }

        static string? OptionalText(JsonElement item, string field, int max)
        {
            if (!TryProperty(item, field, out var value) || value.ValueKind != JsonValueKind.String)
                return null;
            var text = value.GetString()!.Trim();
            return text.Length is > 0 && text.Length <= max ? text : null;
        }

        static decimal? OptionalScore(JsonElement item, string field)
        {
            if (!TryProperty(item, field, out var value))
                return null;

            decimal score;
            if (value.ValueKind == JsonValueKind.Number)
            {
                if (!value.TryGetDecimal(out score)) return null;
            }
            else if (value.ValueKind == JsonValueKind.String)
            {
                var numericText = value.GetString()?.Trim().TrimEnd('%').Trim();
                if (string.IsNullOrEmpty(numericText) ||
                    !decimal.TryParse(numericText.Replace(',', '.'), NumberStyles.Number,
                        CultureInfo.InvariantCulture, out score))
                    return null;
            }
            else return null;

            if (score < 0 || score > 100) return null;
            return score;
        }

        static JsonElement OptionalArray(JsonElement item, string field)
        {
            return TryProperty(item, field, out var value) && value.ValueKind == JsonValueKind.Array
                ? value
                : default;
        }

        root = Unwrap(root);
        var hasSuccess = TryProperty(root, "success", out var success);
        if (hasSuccess && success.ValueKind is not JsonValueKind.True &&
            !(success.ValueKind == JsonValueKind.String && success.GetString()!.Equals("true", StringComparison.OrdinalIgnoreCase)))
            throw Invalid();
        if (!hasSuccess && !TryProperty(root, "analysis", out _) &&
            !TryProperty(root, "modules", out _) && !TryProperty(root, "detectedSkills", out _))
            throw Invalid();
        if (TryProperty(root, "analysisType", out var analysisType) &&
            (analysisType.ValueKind != JsonValueKind.String ||
             !analysisType.GetString()!.Equals("academic_transcript", StringComparison.OrdinalIgnoreCase)))
            throw Invalid();
        var analysis = TryProperty(root, "analysis", out var analysisValue) &&
                       analysisValue.ValueKind == JsonValueKind.Object
            ? analysisValue
            : root;

        var modules = new List<TranscriptModule>();
        var moduleItems = OptionalArray(analysis, "modules");
        if (moduleItems.ValueKind == JsonValueKind.Array)
        {
            foreach (var module in moduleItems.EnumerateArray().Take(300))
            {
                if (module.ValueKind != JsonValueKind.Object) continue;
                var name = OptionalText(module, "moduleName", 200);
                var evidence = OptionalText(module, "evidence", 3000);
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(evidence)) continue;

                var mark = OptionalScore(module, "mark");
                // A numeric result must occur as a number in the supporting OCR text; a course code is not evidence of a grade.
                if (mark.HasValue && !Regex.Matches(evidence, @"(?<![\d.])\d{1,3}(?:[.,]\d{1,2})?(?![\d.])").Cast<Match>()
                        .Any(match => decimal.TryParse(match.Value.Replace(',', '.'), System.Globalization.NumberStyles.Number,
                            System.Globalization.CultureInfo.InvariantCulture, out var value) && value == mark.Value))
                    mark = null;
                modules.Add(new(name, mark, evidence));
            }
        }
        modules = modules.GroupBy(module => module.ModuleName, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First()).ToList();

        var skills = new List<TranscriptSkillEvidence>();
        var skillItems = OptionalArray(analysis, "detectedSkills");
        if (skillItems.ValueKind == JsonValueKind.Array)
        {
            foreach (var skill in skillItems.EnumerateArray().Take(100))
            {
                if (skill.ValueKind != JsonValueKind.Object) continue;
                var name = OptionalText(skill, "skillName", 100);
                var evidence = OptionalText(skill, "evidence", 3000);
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(evidence))
                    skills.Add(new(name, evidence));
            }
        }

        var strengths = new List<string>();
        var strengthItems = OptionalArray(analysis, "strengths");
        if (strengthItems.ValueKind == JsonValueKind.Array)
            foreach (var strength in strengthItems.EnumerateArray().Take(30))
            {
                if (strength.ValueKind != JsonValueKind.String) continue;
                var text = strength.GetString()!.Trim();
                if (text.Length is > 0 and <= 2000) strengths.Add(text);
            }

        var academicScore = OptionalScore(analysis, "academicScore");
        if (!modules.Any(module => module.Mark.HasValue)) academicScore = null;
        return new(academicScore, OptionalText(analysis, "institution", 300),
            OptionalText(analysis, "qualification", 300), OptionalText(analysis, "summary", 5000) ?? "",
            modules, skills, strengths);
    }
}
