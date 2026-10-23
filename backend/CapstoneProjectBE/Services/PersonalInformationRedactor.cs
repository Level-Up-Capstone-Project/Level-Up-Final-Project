using System.Text.RegularExpressions;

namespace CapstoneProjectBE.Services;

public static partial class PersonalInformationRedactor
{
    private const string Redacted = "[REDACTED]";

    [GeneratedRegex(@"(?im)^\s*(?:full\s+name|first\s+name|surname|last\s+name|name|e-?mail|phone(?:\s+number)?|mobile|cell(?:phone)?|contact(?:\s+number)?|(?:national\s+)?id(?:entity)?(?:\s+number)?|student\s+(?:number|no\.?|id)|passport(?:\s+number)?|date\s+of\s+birth|dob|(?:physical|postal|residential)?\s*address(?:\s+(?:line|physical|postal|residential))?)\s*[:#-]\s*.*$", RegexOptions.CultureInvariant)]
    private static partial Regex LabeledPersonalInformation();

    [GeneratedRegex(@"(?<![\w.+-])[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}(?![\w.-])", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex EmailAddress();

    [GeneratedRegex(@"(?<!\d)\d{13}(?!\d)", RegexOptions.CultureInvariant)]
    private static partial Regex NationalId();

    [GeneratedRegex(@"(?<!\d)(?:\+?27|0)\s*\(?\d{2,3}\)?(?:[\s.-]*\d){7,8}(?!\d)", RegexOptions.CultureInvariant)]
    private static partial Regex SouthAfricanPhoneNumber();

    [GeneratedRegex(@"(?im)^\s*(?=[^\r\n]{1,180}$)(?=[^\r\n]*\d)[^\r\n]*\b(?:street|st\.?|road|rd\.?|avenue|ave\.?|drive|dr\.?|lane|ln\.?|crescent|close|boulevard|blvd\.?|highway|hwy\.?)\b[^\r\n]*$", RegexOptions.CultureInvariant)]
    private static partial Regex StreetAddressLine();

    public static string Redact(string text, IEnumerable<string?>? knownIdentifiers = null)
    {
        ArgumentNullException.ThrowIfNull(text);
        var redacted = LabeledPersonalInformation().Replace(text, Redacted);
        redacted = EmailAddress().Replace(redacted, Redacted);
        redacted = NationalId().Replace(redacted, Redacted);
        redacted = SouthAfricanPhoneNumber().Replace(redacted, Redacted);
        redacted = StreetAddressLine().Replace(redacted, Redacted);

        foreach (var identifier in (knownIdentifiers ?? []).Where(value => !string.IsNullOrWhiteSpace(value))
                     .Select(value => value!.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).OrderByDescending(value => value.Length))
        {
            if (identifier.Length < 3) continue;
            var escaped = Regex.Escape(identifier);
            redacted = Regex.Replace(redacted, $@"(?<![\p{{L}}\p{{N}}]){escaped}(?![\p{{L}}\p{{N}}])", Redacted,
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        return redacted;
    }

    public static string RedactFileName(string fileName) =>
        Path.GetExtension(fileName).ToLowerInvariant() is ".pdf" ? "document.pdf" : "document";
}
