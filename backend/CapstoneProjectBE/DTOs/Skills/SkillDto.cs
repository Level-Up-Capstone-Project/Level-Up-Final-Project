namespace CapstoneProjectBE.DTOs.Skills
{
    public class SkillDto
    {
        public Guid SkillID { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
