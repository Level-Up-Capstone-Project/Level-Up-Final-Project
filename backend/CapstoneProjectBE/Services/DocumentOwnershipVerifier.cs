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

        var email = FindDocumentEmail(documentText);
        if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(owner.Email) &&
            !string.Equals(email, owner.Email.Trim(), StringComparison.OrdinalIgnoreCase))
            throw new DocumentOwnershipException(documentType);

        var labelledName = LabelledName().Match(documentText);
        if (labelledName.Success && !NameMatchesOwner(labelledName.Groups["name"].Value, owner))
            throw new DocumentOwnershipException(documentType);
    }

    private static bool NameMatchesOwner(string documentName, DocumentOwner owner)
    {
        var firstName = Normalize(owner.FirstName);
        var lastName = Normalize(owner.LastName);
        var name = Normalize(documentName);
        var nameWords = documentName.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

        // A one-word or abbreviated name is not reliable enough to reject a document.
        if (name.Length == 0 || nameWords.Length < 2 || (firstName.Length == 0 && lastName.Length == 0)) return true;
        return (firstName.Length == 0 || name.Contains(firstName, StringComparison.Ordinal)) &&
               (lastName.Length == 0 || name.Contains(lastName, StringComparison.Ordinal));
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

    [GeneratedRegex(@"(?im)^\s*(?:full\s+name|student\s+name|candidate\s+name|name)\s*[:#-]\s*(?<name>[^\r\n]+?)\s*$", RegexOptions.CultureInvariant)]
    private static partial Regex LabelledName();
}

/// <summary>Trusted account identity used only for a local document ownership check.</summary>
public sealed record DocumentOwner(string? Email, string? FirstName, string? LastName);

/// <summary>Raised when an identifiable document belongs to a different account.</summary>
public sealed class DocumentOwnershipException(string documentType) : Exception(
    $"This {documentType.ToLowerInvariant()} does not match the signed-in account. Upload your own document and try again.");
