namespace CapstoneProjectBE.DTOs.Jobs
{
    public class JobSkillDto
    {
        public Guid SkillID { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string RequirementType { get; set; } = string.Empty;

        public int ImportanceLevel { get; set; }
    }
}