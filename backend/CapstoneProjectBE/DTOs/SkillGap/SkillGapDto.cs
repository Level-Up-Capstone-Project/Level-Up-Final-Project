namespace CapstoneProjectBE.DTOs.SkillGap
{
    public class SkillGapDto
    {
        public SkillGapCareerDto Career { get; set; } = new();
        public decimal SkillMatchPercentage { get; set; }
        public IReadOnlyList<SkillGapItemDto> Skills { get; set; } = new List<SkillGapItemDto>();
    }
}
