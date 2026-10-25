namespace CapstoneProjectBE.Models
{
    /// <summary>
    /// Represents the persisted Job Match domain entity.
    /// </summary>
    public class JobMatch
    {
        public Guid JobMatchID { get; set; }

        public Guid UserID { get; set; }
        public User User { get; set; } = null!;

        public Guid JobID { get; set; }
        public Job Job { get; set; } = null!;

        public decimal SkillMatchScore { get; set; }
        public decimal EducationMatchScore { get; set; }
        public decimal ExperienceMatchScore { get; set; }
        public decimal CareerAlignmentScore { get; set; }
        public decimal OverallMatchScore { get; set; }
        public DateTime CalculatedAt { get; set; }
    }
}
