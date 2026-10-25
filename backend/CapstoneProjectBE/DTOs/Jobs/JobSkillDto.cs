namespace CapstoneProjectBE.DTOs.Jobs
{
    /// <summary>
    /// Carries Job Skill data between API clients and application services.
    /// </summary>
    public class JobSkillDto
    {
        public Guid SkillID { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string RequirementType { get; set; } = string.Empty;

        public int ImportanceLevel { get; set; }
    }
}
