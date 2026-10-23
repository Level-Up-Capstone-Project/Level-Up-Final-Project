namespace CapstoneProjectBE.ExternalServices.CVAnalysis
{
    public interface ICVAnalysisProvider
    {
        void EnsureConfigured() { }
        Task<CVAnalysisResult> AnalyseAsync(CVAnalysisRequest request, CancellationToken ct);
    }

    public record CVAnalysisRequest(Guid RequestId, Guid CVID, Guid? CareerID, string? CareerName,
        string FileName, string ContentType, byte[] Document, IReadOnlyList<CVSkillCandidate> KnownSkills);
}
