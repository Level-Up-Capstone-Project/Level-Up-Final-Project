using System.Text.Json;

namespace CapstoneProjectBE.Services;

/// <summary>
/// Coordinates N8n Response Parser application behavior.
/// </summary>
public static class N8nResponseParser
{
    public static JsonElement Unwrap(JsonElement root)
    {
        for (var depth = 0; depth < 6; depth++)
        {
            if (root.ValueKind == JsonValueKind.Array)
            {
                if (root.GetArrayLength() != 1)
                    throw new N8nAnalysisException("The analysis service returned an unexpected response envelope.");
                root = root[0];
            }

            if (root.ValueKind != JsonValueKind.Object)
                throw new N8nAnalysisException("The analysis service returned an unexpected response envelope.");

            if (HasAny(root, "success", "analysis", "roadmap", "assessment", "message"))
                return root;

            if (TryGetProperty(root, "statusCode", out var statusCode))
            {
                if (statusCode.ValueKind != JsonValueKind.Number ||
                    !statusCode.TryGetInt32(out var status) || status is < 200 or >= 300)
                    throw new N8nAnalysisException("The analysis service returned an unsuccessful response.");
            }

            if (TryGetProperty(root, "json", out var json) && json.ValueKind == JsonValueKind.Object)
            {
                root = json;
                continue;
            }

            var wrapperFound = false;
            foreach (var wrapper in new[] {"response", "body", "data", "output", "result"})
            {
                if (!TryGetProperty(root, wrapper, out var nested) ||
                    nested.ValueKind is not (JsonValueKind.Object or JsonValueKind.Array))
                    continue;
                root = nested;
                wrapperFound = true;
                break;
            }

            if (!wrapperFound)
                return root;
        }

        throw new N8nAnalysisException("The analysis service returned a response with too many nested wrappers.");
    }

    public static bool TryGetProperty(JsonElement item, string name, out JsonElement value)
    {
        if (item.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in item.EnumerateObject())
            {
                if (property.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    value = property.Value;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }

    private static bool HasAny(JsonElement item, params string[] names) =>
        names.Any(name => TryGetProperty(item, name, out _));
}
