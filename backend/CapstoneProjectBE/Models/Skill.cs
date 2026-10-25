namespace CapstoneProjectBE.Models
{
    /// <summary>
    /// Represents the persisted Skill domain entity.
    /// </summary>
    public class Skill
    {
        public Guid SkillID { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<CareerSkill> CareerSkills { get; set; }
            = new List<CareerSkill>();
    }
}
