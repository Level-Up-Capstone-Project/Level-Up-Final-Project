using System.Text.Json;
using CapstoneProjectBE.Data;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Services;

internal static class TranscriptSkillEvidenceReader
{
    public static async Task<HashSet<Guid>> GetLatestAsync(AppDbContext context, Guid userId, CancellationToken ct)
    {
        var json = await context.TranscriptAnalyses.AsNoTracking().Where(item => item.UserID == userId)
            .OrderByDescending(item => item.AnalysedAt).ThenByDescending(item => item.AnalysisID)
            .Select(item => item.ResultJson).FirstOrDefaultAsync(ct);
        if (json is null) return [];
        try
        {
            using var document = JsonDocument.Parse(json);
            if (!document.RootElement.TryGetProperty("detectedSkills", out var skills) || skills.ValueKind != JsonValueKind.Array) return [];
            return skills.EnumerateArray().Where(skill => skill.ValueKind == JsonValueKind.Object &&
                    skill.TryGetProperty("skillID", out var id) && id.ValueKind == JsonValueKind.String && Guid.TryParse(id.GetString(), out _))
                .Select(skill => Guid.Parse(skill.GetProperty("skillID").GetString()!)).ToHashSet();
        }
        catch (JsonException) { return []; }
    }
}
