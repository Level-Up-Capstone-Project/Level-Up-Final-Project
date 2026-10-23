namespace CapstoneProjectBE.ExternalServices.CVAnalysis
{
    public class CVAnalysisResult
    {
        public decimal QualityScore { get; set; }
        public decimal ATSScore { get; set; }
        public decimal FormattingScore { get; set; }
        public string Strengths { get; set; } = string.Empty;
        public string Weaknesses { get; set; } = string.Empty;
        public string Recommendations { get; set; } = string.Empty;
        public IReadOnlyList<CVSkillCandidate> DetectedSkills { get; set; } = Array.Empty<CVSkillCandidate>();
    }
}
