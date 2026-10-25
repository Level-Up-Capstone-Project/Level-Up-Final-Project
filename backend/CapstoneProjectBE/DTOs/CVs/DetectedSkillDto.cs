namespace CapstoneProjectBE.DTOs.CVs
{
    /// <summary>
    /// Carries Detected Skill data between API clients and application services.
    /// </summary>
    public class DetectedSkillDto
    {
        public Guid SkillID { get; set; }
        public string SkillName { get; set; } = string.Empty;
    }
}
