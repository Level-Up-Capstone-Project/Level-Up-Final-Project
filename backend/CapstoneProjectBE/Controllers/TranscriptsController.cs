using System.Security.Claims;
using System.Text.Json;
using CapstoneProjectBE.DTOs.Transcripts;
using CapstoneProjectBE.ExternalServices.Transcripts;
using CapstoneProjectBE.ExternalServices.CVAnalysis;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Data;
using CapstoneProjectBE.ExternalServices.Storage;
using CapstoneProjectBE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;

namespace CapstoneProjectBE.Controllers;

[ApiController, Authorize, Route("api/transcripts")]
public class TranscriptsController(AppDbContext db, ITranscriptAnalysisProvider workflow) : ControllerBase
{
    [EnableRateLimiting("ai-workflow")]
    [HttpPost("analyse"), HttpPost("analyze"), Consumes("multipart/form-data")]
    [RequestSizeLimit(6 * 1024 * 1024), RequestFormLimits(MultipartBodyLengthLimit = 6 * 1024 * 1024)]
    public async Task<IActionResult> Analyse(IFormFile file, CancellationToken ct)
    {
        if (Request.Headers.Accept.ToString().Contains("application/x-ndjson", StringComparison.OrdinalIgnoreCase))
        {
            await AnalysisStream.WriteAsync(HttpContext, progress => AnalyseCore(file, ct, progress));
            return new EmptyResult();
        }
        return await AnalyseCore(file, ct);
    }

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    [HttpGet("current/analysis")]
    public async Task<IActionResult> Current(CancellationToken ct)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ||
            !await db.Users.AnyAsync(user => user.UserID == userId && user.IsActive, ct)) return Unauthorized();
        var saved = await db.TranscriptAnalyses.AsNoTracking().Where(item => item.UserID == userId)
            .OrderByDescending(item => item.AnalysedAt).ThenByDescending(item => item.AnalysisID).FirstOrDefaultAsync(ct);
        return saved is null ? NotFound() : Ok(JsonSerializer.Deserialize<TranscriptAnalysisDto>(saved.ResultJson, Json));
    }

    private async Task<IActionResult> AnalyseCore(IFormFile file, CancellationToken ct, Func<int, Task>? progress = null)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ||
            !await db.Users.AnyAsync(user => user.UserID == userId && user.IsActive, ct)) return Unauthorized();
        try
        {
            if (!string.Equals(Path.GetExtension(file.FileName), ".pdf", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Choose a PDF transcript.");
            await CVStorageService.ValidateFileAsync(file, ct);
            if (file.FileName.Length > 255) throw new InvalidDataException("Choose a transcript with a shorter file name.");
            workflow.EnsureConfigured();
            if (progress is not null) await progress(15);
            using var bytes = new MemoryStream();
            await file.CopyToAsync(bytes, ct);
            if (progress is not null) await progress(25);
            if (progress is not null) await progress(65);
            var careerName = await db.Users.AsNoTracking().Where(user => user.UserID == userId)
                .Select(user => new
                {
                    CareerName = user.UserProfile == null || user.UserProfile.TargetCareer == null ? null : user.UserProfile.TargetCareer.CareerName,
                }).Select(profile => profile.CareerName).SingleOrDefaultAsync(ct);
            var safeFileName = Path.GetFileName(file.FileName.Replace('\\', '/'));
            var catalogue = await db.Skills.AsNoTracking().Select(skill => new {skill.SkillID, skill.SkillName}).ToListAsync(ct);
            var insights = await workflow.AnalyseAsync(careerName, safeFileName, "application/pdf", bytes.ToArray(), ct,
                catalogue.Select(skill => skill.SkillName).ToList());
            if (progress is not null) await progress(85);
            var subjects = insights.Modules.Where(module => module.Mark.HasValue)
                .Select(module => new TranscriptSubject(module.ModuleName, module.Mark!.Value)).ToArray();
            decimal? averageMark = subjects.Length == 0 ? null : Math.Round(subjects.Average(subject => subject.Mark), 2);
            var skills = catalogue.Where(skill => insights.DetectedSkills.Any(detected =>
                    SkillNameMatcher.NamesMatch(skill.SkillName, detected.SkillName)))
                .Select(skill => new TranscriptDetectedSkillDto(skill.SkillID, skill.SkillName)).ToArray();
            var result = new TranscriptAnalysisDto(Guid.NewGuid(), DateTime.UtcNow, safeFileName, file.Length, careerName,
                string.IsNullOrWhiteSpace(insights.Summary) ? $"Analysed {insights.Modules.Count} modules. Review extracted marks against your transcript." : insights.Summary,
                0, averageMark, insights.AcademicScore, insights.Institution, insights.Qualification,
                insights.Modules, subjects, skills, insights.DetectedSkills, insights.Strengths);
            ct.ThrowIfCancellationRequested();
            db.TranscriptAnalyses.Add(new TranscriptAnalysis {AnalysisID = result.AnalysisID, UserID = userId,
                AnalysedAt = result.AnalysedAt, ResultJson = JsonSerializer.Serialize(result, Json)});
            await db.SaveChangesAsync(ct);
            if (progress is not null) await progress(95);
            return Ok(result);
        }
        catch (N8nAnalysisException error) { return Problem(title: error.Message, statusCode: error.StatusCode); }
        catch (InvalidDataException error) { return BadRequest(new {message = error.Message}); }
    }
}
