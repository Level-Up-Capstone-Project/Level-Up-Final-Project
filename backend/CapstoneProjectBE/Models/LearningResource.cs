//details of external courses, videos, or articles

namespace CapstoneProjectBE.Models
{
    /// <summary>
    /// Represents the persisted Learning Resource domain entity.
    /// </summary>
    public class LearningResource
    {
        public Guid ResourceID { get; set; }

        public Guid SkillID { get; set; }
        public Skill Skill { get; set; } = null!;

        public string ResourceName { get; set; } = string.Empty;

        public string ResourceType { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public string Provider { get; set; } = string.Empty;

        public string DifficultyLevel { get; set; } = string.Empty;

        public string CostType { get; set; } = string.Empty;

        public string EstDuration { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string SourceType { get; set; } = string.Empty; 

        public bool IsActive { get; set; } = true; 

    }
}
