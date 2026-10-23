namespace CapstoneProjectBE.DTOs.SkillGap
{
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
