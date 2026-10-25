namespace CapstoneProjectBE.DTOs.SkillGap
{
    /// <summary>
    /// Carries Skill Gap Career data between API clients and application services.
    /// </summary>
    public class SkillGapCareerDto
    {
        public Guid CareerID { get; set; }
        public string CareerName { get; set; } = string.Empty;
    }
}
