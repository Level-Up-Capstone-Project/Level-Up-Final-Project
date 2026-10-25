namespace CapstoneProjectBE.ExternalServices.Transcripts;

/// <summary>
/// Carries Transcript Module values used by an external integration.
/// </summary>
public record TranscriptModule(string ModuleName, decimal? Mark, string Evidence);
/// <summary>
/// Carries Transcript Subject values used by an external integration.
/// </summary>
public record TranscriptSubject(string Subject, decimal Mark);
/// <summary>
/// Carries Transcript Skill Evidence values used by an external integration.
/// </summary>
public record TranscriptSkillEvidence(string SkillName, string Evidence);
/// <summary>
/// Carries Transcript Insights values used by an external integration.
/// </summary>
public record TranscriptInsights(decimal? AcademicScore, string? Institution, string? Qualification,
    string Summary, IReadOnlyList<TranscriptModule> Modules, IReadOnlyList<TranscriptSkillEvidence> DetectedSkills,
    IReadOnlyList<string> Strengths);

/// <summary>
/// Defines the contract for the Transcript Analysis Provider integration.
/// </summary>
public interface ITranscriptAnalysisProvider
{
    void EnsureConfigured() { }
    Task<TranscriptInsights> AnalyseAsync(string? careerName, string fileName, string contentType,
        byte[] document, CancellationToken ct, IReadOnlyList<string>? knownSkills = null);
}
