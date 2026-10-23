namespace CapstoneProjectBE.Models
{
    public class CVAnalysisSkill
    {
        public Guid CVAnalysisSkillID { get; set; }
        public Guid AnalysisID { get; set; }
        public Guid SkillID { get; set; }
        public CVAnalysis Analysis { get; set; } = null!;
        public Skill Skill { get; set; } = null!;
    }
}
