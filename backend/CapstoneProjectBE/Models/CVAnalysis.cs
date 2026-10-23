namespace CapstoneProjectBE.Models
{
    public class CVAnalysis
    {
        public Guid AnalysisID { get; set; }
        public Guid CVID { get; set; }
        public decimal QualityScore { get; set; }
        public decimal ATSScore { get; set; }
        public decimal FormattingScore { get; set; }
        public string Strengths { get; set; } = string.Empty;
        public string Weaknesses { get; set; } = string.Empty;
        public string Recommendations { get; set; } = string.Empty;
        public DateTime AnalysedAt { get; set; }
        public CV CV { get; set; } = null!;
        public ICollection<CVAnalysisSkill> DetectedSkills { get; set; } = new List<CVAnalysisSkill>();
    }
}
