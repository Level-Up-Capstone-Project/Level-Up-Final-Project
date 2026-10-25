namespace CapstoneProjectBE.ExternalServices.CVAnalysis
{
    /// <summary>
    /// Carries CVSkill Candidate values used by an external integration.
    /// </summary>
    public sealed record CVSkillCandidate(Guid SkillID, string SkillName);
}
