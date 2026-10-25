namespace CapstoneProjectBE.DTOs.SkillGap
{
    /// <summary>
    /// Carries Skill Gap Item data between API clients and application services.
    /// </summary>
    public class SkillGapItemDto
    {
        public Guid SkillID { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int ImportanceLevel { get; set; }
        public bool IsCoreSkill { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
