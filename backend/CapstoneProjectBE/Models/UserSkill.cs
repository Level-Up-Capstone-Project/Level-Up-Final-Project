namespace CapstoneProjectBE.Models
{
    public class UserSkill
    {
        public Guid UserSkillID { get; set; }
        public Guid UserID { get; set; }
        public Guid SkillID { get; set; }
        public string Status { get; set; } = "missing";
        public bool IsSelfReported { get; set; }
        public bool IsCVDetected { get; set; }
        public bool IsKnowledgeVerified { get; set; }
        public DateTime UpdatedAt { get; set; }
        public User User { get; set; } = null!;
        public Skill Skill { get; set; } = null!;
    }
}
