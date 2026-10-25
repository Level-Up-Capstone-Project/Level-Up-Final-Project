namespace CapstoneProjectBE.Models
{
    /// <summary>
    /// Represents the persisted CVAnalysis Skill domain entity.
    /// </summary>
    public class CVAnalysisSkill
    {
        public Guid CVAnalysisSkillID { get; set; }
        public Guid AnalysisID { get; set; }
        public Guid SkillID { get; set; }
        public CVAnalysis Analysis { get; set; } = null!;
        public Skill Skill { get; set; } = null!;
    }
}
