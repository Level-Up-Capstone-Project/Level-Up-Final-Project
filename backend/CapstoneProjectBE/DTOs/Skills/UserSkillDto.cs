namespace CapstoneProjectBE.DTOs.Skills
{
    /// <summary>
    /// Carries User Skill data between API clients and application services.
    /// </summary>
    public class UserSkillDto
    {
        public Guid UserSkillID { get; set; }
        public Guid SkillID { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsSelfReported { get; set; }
        public bool IsCVDetected { get; set; }
        public bool IsKnowledgeVerified { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
