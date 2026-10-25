namespace CapstoneProjectBE.DTOs.SkillGap
{
    /// <summary>
    /// Carries Skill Gap data between API clients and application services.
    /// </summary>
    public class SkillGapDto
    {
        public SkillGapCareerDto Career { get; set; } = new();
        public decimal SkillMatchPercentage { get; set; }
        public IReadOnlyList<SkillGapItemDto> Skills { get; set; } = new List<SkillGapItemDto>();
    }
}
