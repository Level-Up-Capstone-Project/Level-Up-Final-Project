using CapstoneProjectBE.Services;

namespace CapstoneProjectBE.ExternalServices.CVAnalysis
{
    /// <summary>
    /// Defines the contract for the CVAnalysis Provider integration.
    /// </summary>
    public interface ICVAnalysisProvider
    {
        void EnsureConfigured() { }
        Task<CVAnalysisResult> AnalyseAsync(CVAnalysisRequest request, CancellationToken ct);
    }

    /// <summary>
    /// Carries CVAnalysis Request values used by an external integration.
    /// </summary>
    public record CVAnalysisRequest(Guid RequestId, Guid CVID, Guid? CareerID, string? CareerName,
        string FileName, string ContentType, byte[] Document, IReadOnlyList<CVSkillCandidate> KnownSkills,
        DocumentOwner? Owner = null);
}
