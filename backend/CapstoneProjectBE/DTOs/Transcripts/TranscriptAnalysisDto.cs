using CapstoneProjectBE.ExternalServices.Transcripts;
using CapstoneProjectBE.Services;

namespace CapstoneProjectBE.DTOs.Transcripts;

/// <summary>
/// Carries Transcript Detected Skill data between API clients and application services.
/// </summary>
public record TranscriptDetectedSkillDto(Guid SkillID, string SkillName);
/// <summary>
/// Carries Transcript Analysis data between API clients and application services.
/// </summary>
public record TranscriptAnalysisDto(Guid AnalysisID, DateTime AnalysedAt, string FileName, long FileSize,
    string? CareerName, string Summary, int TextCharacters, decimal? AverageMark, decimal? AcademicScore,
    string? Institution, string? Qualification, IReadOnlyList<TranscriptModule> Modules, IReadOnlyList<TranscriptSubject> Subjects,
    IReadOnlyList<TranscriptDetectedSkillDto> DetectedSkills, IReadOnlyList<TranscriptSkillEvidence> SkillEvidence,
    IReadOnlyList<string> Strengths, string Provider = "n8n");
