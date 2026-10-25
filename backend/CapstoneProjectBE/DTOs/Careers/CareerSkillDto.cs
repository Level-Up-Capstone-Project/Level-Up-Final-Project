namespace CapstoneProjectBE.DTOs.Careers
{
    /// <summary>
    /// Carries Career Skill data between API clients and application services.
    /// </summary>
    public class CareerSkillDto
    {
        public Guid SkillID { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int ImportanceLevel { get; set; }
        public bool IsCoreSkill { get; set; }
    }
}
