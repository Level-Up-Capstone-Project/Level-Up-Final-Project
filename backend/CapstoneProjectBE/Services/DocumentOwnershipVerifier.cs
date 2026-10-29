using System.Text.RegularExpressions;

namespace CapstoneProjectBE.Services;

/// <summary>
/// Checks document identity in application memory before personal data is redacted
/// and sent to an external analysis workflow.
/// </summary>
public static partial class DocumentOwnershipVerifier
{
    public static void EnsureMatches(DocumentOwner? owner, string documentText, string documentType)
    {
        if (owner is null || string.IsNullOrWhiteSpace(documentText)) return;

        if (!HeaderContainsProfileName(documentText, owner))
            throw new DocumentOwnershipException(documentType);

        var email = FindDocumentEmail(documentText);
        if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(owner.Email) &&
            !string.Equals(email, owner.Email.Trim(), StringComparison.OrdinalIgnoreCase))
            throw new DocumentOwnershipException(documentType);

    }

    private static bool HeaderContainsProfileName(string documentText, DocumentOwner owner)
    {
        var expectedNameTokens = NameToken().Matches($"{owner.FirstName} {owner.LastName}")
            .Select(match => Normalize(match.Value)).Where(token => token.Length > 1).Distinct().ToArray();
        if (expectedNameTokens.Length < 2) return false;

        // Identity blocks are expected at the top of both CVs and academic transcripts.
        // Restricting this to the header prevents a reference or lecturer later in the document
        // from being treated as the owner.
        var header = documentText[..Math.Min(documentText.Length, 6000)];
        var headerTokens = NameToken().Matches(header).Select(match => Normalize(match.Value)).ToHashSet();
        return expectedNameTokens.All(headerTokens.Contains);
    }

    private static string Normalize(string? value) => new((value ?? string.Empty)
        .Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());

    private static string? FindDocumentEmail(string documentText)
    {
        var labelled = LabelledEmail().Match(documentText);
        if (labelled.Success) return labelled.Groups["email"].Value.Trim();

        // CVs commonly place the owner's email on its own line in the header without a label.
        // Limit inspection to the header so references/recruiters later in the document do not affect ownership.
        foreach (var line in documentText.Replace("\r", string.Empty).Split('\n').Take(12))
        {
            var bare = BareEmail().Match(line.Trim());
            if (bare.Success) return bare.Groups["email"].Value;
        }
        return null;
    }

    [GeneratedRegex(@"(?im)^\s*(?:e-?mail|email\s+address)\s*[:#-]\s*(?<email>[a-z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?(?:\.[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?)+)\s*$", RegexOptions.CultureInvariant)]
    private static partial Regex LabelledEmail();

    [GeneratedRegex(@"^(?<email>[a-z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?(?:\.[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?)+)$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex BareEmail();

    [GeneratedRegex(@"[\p{L}\p{N}]+", RegexOptions.CultureInvariant)]
    private static partial Regex NameToken();
}

/// <summary>Trusted account identity used only for a local document ownership check.</summary>
public sealed record DocumentOwner(string? Email, string? FirstName, string? LastName);

/// <summary>Raised when an identifiable document belongs to a different account.</summary>
public sealed class DocumentOwnershipException(string documentType) : Exception(
    $"This {documentType.ToLowerInvariant()} does not match the signed-in account. Upload your own document and try again.");
