namespace CapstoneProjectBE.DTOs.Skills
{
    /// <summary>
    /// Carries Skill data between API clients and application services.
    /// </summary>
    public class SkillDto
    {
        public Guid SkillID { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
