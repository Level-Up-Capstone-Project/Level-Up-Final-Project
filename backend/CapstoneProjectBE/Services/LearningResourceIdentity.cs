using System.Globalization;
using System.Text;

namespace CapstoneProjectBE.Services;

/// <summary>
/// Normalizes provider identifiers and public HTTPS URLs used to deduplicate discovered learning resources.
/// </summary>
public static class LearningResourceIdentity
{
    public static string ProviderKey(string provider)
    {
        var normalized = provider.Trim().Normalize(NormalizationForm.FormKD);
        var key = new StringBuilder(normalized.Length);
        var separatorPending = false;

        foreach (char character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(character))
            {
                if (separatorPending && key.Length > 0)
                {
                    key.Append('-');
                }

                key.Append(char.ToLowerInvariant(character));
                separatorPending = false;
            }
            else
            {
                separatorPending = key.Length > 0;
            }
        }

        return key.ToString();
    }

    public static bool TryCanonicalizeHttpsUrl(string value, out string canonicalUrl)
    {
        canonicalUrl = string.Empty;
        if (!Uri.TryCreate(value.Trim(), UriKind.Absolute, out Uri? uri) ||
            !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ||
            !string.IsNullOrEmpty(uri.UserInfo) ||
            uri.HostNameType != UriHostNameType.Dns ||
            string.Equals(uri.Host, "localhost", StringComparison.OrdinalIgnoreCase) ||
            uri.Host.EndsWith(".local", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        try
        {
            var builder = new UriBuilder(uri)
            {
                Scheme = Uri.UriSchemeHttps,
                Host = uri.IdnHost.ToLowerInvariant(),
                Port = -1,
                Fragment = string.Empty
            };

            canonicalUrl = builder.Uri.AbsoluteUri;
            if (builder.Uri.Query.Length == 0 && builder.Uri.AbsolutePath.Length > 1)
            {
                canonicalUrl = canonicalUrl.TrimEnd('/');
            }

            return canonicalUrl.Length <= 500;
        }
        catch (UriFormatException)
        {
            canonicalUrl = string.Empty;
            return false;
        }
    }
}
