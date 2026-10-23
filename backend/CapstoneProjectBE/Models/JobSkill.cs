namespace CapstoneProjectBE.Models
{
    //links job to skill it requires
    public class JobSkill
    {
        
        public Guid JobSkillID { get; set; }

        public Guid JobID { get; set; }
        public Job Job { get; set; } = null!;

        public Guid SkillID { get; set; }
        public Skill Skill { get; set; } = null!;

        public string RequirementType { get; set; } = string.Empty;
        public int ImportanceLevel { get; set; }
    }
}