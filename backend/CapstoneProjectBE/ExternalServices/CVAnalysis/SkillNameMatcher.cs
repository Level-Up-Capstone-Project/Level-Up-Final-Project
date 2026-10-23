using System.Text.RegularExpressions;

namespace CapstoneProjectBE.ExternalServices.CVAnalysis;

/// <summary>Matches canonical catalogue skills against common CV and workflow spelling variants.</summary>
public static partial class SkillNameMatcher
{
    public static bool NamesMatch(string left, string right) => Normalize(left) == Normalize(right);

    public static bool TextContains(string text, string skillName)
    {
        var normalizedText = $" {Normalize(text)} ";
        var normalizedSkill = Normalize(skillName);
        return normalizedSkill.Length >= 2 && normalizedText.Contains($" {normalizedSkill} ", StringComparison.Ordinal);
    }

    private static string Normalize(string value)
    {
        var normalized = value.ToLowerInvariant();
        normalized = CSharp().Replace(normalized, " csharp ");
        normalized = CPlusPlus().Replace(normalized, " cplusplus ");
        normalized = DotNet().Replace(normalized, " dotnet ");
        normalized = NodeJs().Replace(normalized, " nodejs ");
        normalized = ReactJs().Replace(normalized, " react ");
        normalized = JavaScript().Replace(normalized, " javascript ");
        normalized = TypeScript().Replace(normalized, " typescript ");
        normalized = PostgreSql().Replace(normalized, " postgresql ");
        normalized = HtmlVersion().Replace(normalized, " html ");
        normalized = CssVersion().Replace(normalized, " css ");
        normalized = PowerBi().Replace(normalized, " powerbi ");
        normalized = Restful().Replace(normalized, "rest");
        normalized = ApiPlural().Replace(normalized, "api");
        normalized = NonAlphaNumeric().Replace(normalized, " ");
        return Whitespace().Replace(normalized, " ").Trim();
    }

    [GeneratedRegex(@"(?<![a-z0-9])c\s*(?:#|sharp)(?![a-z0-9])", RegexOptions.CultureInvariant)]
    private static partial Regex CSharp();

    [GeneratedRegex(@"(?<![a-z0-9])c\s*\+\+(?![a-z0-9])", RegexOptions.CultureInvariant)]
    private static partial Regex CPlusPlus();

    [GeneratedRegex(@"(?<![a-z0-9])\.?net(?![a-z0-9])", RegexOptions.CultureInvariant)]
    private static partial Regex DotNet();

    [GeneratedRegex(@"(?<![a-z0-9])node[\s.]?js(?![a-z0-9])", RegexOptions.CultureInvariant)]
    private static partial Regex NodeJs();

    [GeneratedRegex(@"(?<![a-z0-9])react(?:\.?js)?(?![a-z0-9])", RegexOptions.CultureInvariant)]
    private static partial Regex ReactJs();

    [GeneratedRegex(@"(?<![a-z0-9])(?:java\s*script|js)(?![a-z0-9])", RegexOptions.CultureInvariant)]
    private static partial Regex JavaScript();

    [GeneratedRegex(@"(?<![a-z0-9])(?:type\s*script|ts)(?![a-z0-9])", RegexOptions.CultureInvariant)]
    private static partial Regex TypeScript();

    [GeneratedRegex(@"(?<![a-z0-9])postgres(?:ql)?(?![a-z0-9])", RegexOptions.CultureInvariant)]
    private static partial Regex PostgreSql();

    [GeneratedRegex(@"(?<![a-z0-9])html\s*5(?![a-z0-9])", RegexOptions.CultureInvariant)]
    private static partial Regex HtmlVersion();

    [GeneratedRegex(@"(?<![a-z0-9])css\s*3(?![a-z0-9])", RegexOptions.CultureInvariant)]
    private static partial Regex CssVersion();

    [GeneratedRegex(@"(?<![a-z0-9])power\s*bi(?![a-z0-9])", RegexOptions.CultureInvariant)]
    private static partial Regex PowerBi();

    [GeneratedRegex(@"\brestful\b", RegexOptions.CultureInvariant)]
    private static partial Regex Restful();

    [GeneratedRegex(@"\bapis\b", RegexOptions.CultureInvariant)]
    private static partial Regex ApiPlural();

    [GeneratedRegex(@"[^a-z0-9]+", RegexOptions.CultureInvariant)]
    private static partial Regex NonAlphaNumeric();

    [GeneratedRegex(@"\s+", RegexOptions.CultureInvariant)]
    private static partial Regex Whitespace();
}
