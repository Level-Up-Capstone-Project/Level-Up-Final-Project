namespace CapstoneProjectBE.Models
{
    /// <summary>
    /// Represents the persisted Career Skill domain entity.
    /// </summary>
    public class CareerSkill
    {
        public Guid CareerSkillID { get; set; }
        public Guid CareerID { get; set; }
        public Guid SkillID { get; set; }
        public int ImportanceLevel { get; set; }
        public bool IsCoreSkill { get; set; }
        public Career Career { get; set; } = null!;
        public Skill Skill { get; set; } = null!;
    }
}
