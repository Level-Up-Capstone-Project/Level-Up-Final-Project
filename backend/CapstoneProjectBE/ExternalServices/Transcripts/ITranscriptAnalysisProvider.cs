namespace CapstoneProjectBE.ExternalServices.Transcripts;

public record TranscriptModule(string ModuleName, decimal? Mark, string Evidence);
public record TranscriptSubject(string Subject, decimal Mark);
public record TranscriptSkillEvidence(string SkillName, string Evidence);
public record TranscriptInsights(decimal? AcademicScore, string? Institution, string? Qualification,
    string Summary, IReadOnlyList<TranscriptModule> Modules, IReadOnlyList<TranscriptSkillEvidence> DetectedSkills,
    IReadOnlyList<string> Strengths);

public interface ITranscriptAnalysisProvider
{
    void EnsureConfigured() { }
    Task<TranscriptInsights> AnalyseAsync(string? careerName, string fileName, string contentType,
        byte[] document, CancellationToken ct, IReadOnlyList<string>? knownSkills = null);
}
