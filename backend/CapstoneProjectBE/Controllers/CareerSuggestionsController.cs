using System.Security.Claims;
using System.Text.Json;
using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.Transcripts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Controllers;

/// <summary>
/// Handles HTTP requests for Career Suggestions operations.
/// </summary>
[Authorize]
[ApiController]
[Route("api/careers/suggestions")]
public sealed class CareerSuggestionsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return Unauthorized();
        var cv = await db.CVAnalyses.AsNoTracking().Where(a => a.CV.UserID == userId && a.CV.IsCurrent)
            .OrderByDescending(a => a.AnalysedAt).Select(a => new {Skills = a.DetectedSkills.Select(s => s.SkillID).ToList()}).FirstOrDefaultAsync(ct);
        var transcriptJson = await db.TranscriptAnalyses.AsNoTracking().Where(t => t.UserID == userId)
            .OrderByDescending(t => t.AnalysedAt).Select(t => t.ResultJson).FirstOrDefaultAsync(ct);
        var transcript = transcriptJson is null ? null : JsonSerializer.Deserialize<TranscriptAnalysisDto>(transcriptJson, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        var cvSkills = (cv?.Skills ?? []).ToHashSet();
        var transcriptSkills = (transcript?.DetectedSkills ?? []).Select(s => s.SkillID).ToHashSet();
        var careers = await db.Careers.AsNoTracking().Select(c => new {c.CareerID, c.CareerName, Skills = c.CareerSkills.Select(s => new {s.SkillID, s.Skill.SkillName, s.ImportanceLevel}).ToList()}).ToListAsync(ct);
        var suggestions = careers.Select(c => new {c.CareerID, c.CareerName,
            MatchedSkills = c.Skills.Where(s => cvSkills.Contains(s.SkillID) || transcriptSkills.Contains(s.SkillID)).Select(s => new {s.SkillName, FromCv = cvSkills.Contains(s.SkillID), FromTranscript = transcriptSkills.Contains(s.SkillID)}).ToList(),
            Weight = c.Skills.Where(s => cvSkills.Contains(s.SkillID) || transcriptSkills.Contains(s.SkillID)).Sum(s => Math.Max(0, s.ImportanceLevel))})
            .Where(c => c.MatchedSkills.Count > 0).OrderByDescending(c => c.Weight).ThenBy(c => c.CareerName).Take(10)
            .Select(c => new {c.CareerID, c.CareerName, c.MatchedSkills});
        return Ok(new {hasCvAnalysis = cv is not null, hasTranscriptAnalysis = transcript is not null, suggestions});
    }
}
