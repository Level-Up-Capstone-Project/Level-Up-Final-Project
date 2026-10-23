using System.Text.RegularExpressions;

namespace CapstoneProjectBE.ExternalServices.CVAnalysis;

/// <summary>Produces a conservative CV assessment from extracted text when the AI workflow is unavailable.</summary>
public sealed class TextCVAnalysisProvider
{
    private static readonly string[] ActionVerbs =
    [
        "achieved", "built", "created", "designed", "developed", "implemented",
        "improved", "increased", "led", "managed", "reduced", "resolved"
    ];

    public CVAnalysisResult Analyse(string cvText, IReadOnlyList<CVSkillCandidate> knownSkills)
    {
        var normalizedText = Regex.Replace(cvText, @"\s+", " ").Trim();
        var lowerText = normalizedText.ToLowerInvariant();
        var wordCount = normalizedText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        var lineCount = cvText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Length;
        var hasEmail = Regex.IsMatch(normalizedText,
            @"\b[A-Z0-9._%+\-]+@[A-Z0-9.\-]+\.[A-Z]{2,}\b", RegexOptions.IgnoreCase);
        var hasPhone = Regex.IsMatch(normalizedText, @"(?:\+?27|0)[\s\-]?\d{2}[\s\-]?\d{3}[\s\-]?\d{4}");
        var hasExperienceSection = HasAny(lowerText, "experience", "work history", "employment");
        var hasEducationSection = HasAny(lowerText, "education", "qualification", "academic");
        var hasSkillsSection = HasAny(lowerText, "skills", "technical skills", "competencies");
        var hasSummarySection = HasAny(lowerText, "summary", "profile", "objective");
        var hasProjectsSection = HasAny(lowerText, "projects", "portfolio");
        var sectionCount = new[]
        {
            hasExperienceSection, hasEducationSection, hasSkillsSection, hasSummarySection, hasProjectsSection
        }.Count(section => section);
        var actionVerbCount = ActionVerbs.Count(verb => ContainsPhrase(normalizedText, verb));
        var detectedSkills = DetectSkills(cvText, knownSkills);

        decimal atsScore = 0;
        if (hasEmail) atsScore += 10;
        if (hasPhone) atsScore += 10;
        if (hasExperienceSection) atsScore += 20;
        if (hasEducationSection) atsScore += 15;
        if (hasSkillsSection) atsScore += 15;
        if (detectedSkills.Count > 0) atsScore += 15;
        if (actionVerbCount >= 3) atsScore += 15;

        decimal formattingScore = 0;
        if (wordCount is >= 300 and <= 1200) formattingScore += 30;
        else if (wordCount is >= 150 and <= 1800) formattingScore += 20;
        else if (wordCount >= 50) formattingScore += 10;
        formattingScore += Math.Min(sectionCount * 8, 40);
        if (lineCount is >= 5 and <= 200) formattingScore += 15;
        var unusualCharacters = normalizedText.Count(character =>
            !char.IsLetterOrDigit(character) && !char.IsWhiteSpace(character) && !".,;:!?@+#()&/%'-".Contains(character));
        var unusualCharacterRatio = normalizedText.Length == 0 ? 1 : (decimal)unusualCharacters / normalizedText.Length;
        if (unusualCharacterRatio < 0.10m) formattingScore += 15;

        var strengths = new List<string>();
        var weaknesses = new List<string>();
        var recommendations = new List<string>();
        if (hasEmail && hasPhone) strengths.Add("Contact information is clearly provided");
        else
        {
            weaknesses.Add("Contact information appears incomplete");
            recommendations.Add("Include a professional email address and phone number");
        }
        if (sectionCount >= 3) strengths.Add("The CV contains several recognised sections");
        else
        {
            weaknesses.Add("The CV has limited clearly identifiable sections");
            recommendations.Add("Use headings such as Summary, Skills, Experience and Education");
        }
        if (detectedSkills.Count > 0) strengths.Add($"{detectedSkills.Count} known skill(s) were identified");
        else
        {
            weaknesses.Add("No skills from the platform catalogue were identified");
            recommendations.Add("Add a dedicated skills section using specific skill names");
        }
        if (actionVerbCount >= 3) strengths.Add("Experience uses achievement-focused action verbs");
        else
        {
            weaknesses.Add("Experience contains few achievement-focused action verbs");
            recommendations.Add("Describe achievements with verbs such as developed, improved or implemented");
        }
        if (wordCount is >= 300 and <= 1200) strengths.Add("The CV has an appropriate amount of detail");
        else
        {
            weaknesses.Add("The CV may be too short or too long");
            recommendations.Add("Aim for a concise CV of roughly 300 to 1200 words");
        }

        return new CVAnalysisResult
        {
            QualityScore = Math.Round(atsScore * 0.60m + formattingScore * 0.40m, 2),
            ATSScore = atsScore,
            FormattingScore = formattingScore,
            Strengths = string.Join("; ", strengths),
            Weaknesses = string.Join("; ", weaknesses),
            Recommendations = string.Join("; ", recommendations),
            DetectedSkills = detectedSkills
        };
    }

    public IReadOnlyList<CVSkillCandidate> DetectSkills(string text, IReadOnlyList<CVSkillCandidate> knownSkills) =>
        knownSkills.Where(skill => SkillNameMatcher.TextContains(text, skill.SkillName))
            .DistinctBy(skill => skill.SkillID)
            .OrderBy(skill => skill.SkillName)
            .ToList();

    private static bool HasAny(string text, params string[] terms) => terms.Any(term => ContainsPhrase(text, term));

    private static bool ContainsPhrase(string text, string phrase)
    {
        var pattern = $@"(?<![\p{{L}}\p{{N}}]){Regex.Escape(phrase)}(?![\p{{L}}\p{{N}}])";
        return Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }
}
