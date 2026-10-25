namespace CapstoneProjectBE.Models
{
    /// <summary>
    /// Represents the persisted Skill Assessment domain entity.
    /// </summary>
    public class SkillAssessment
    {
        public Guid SkillAssessmentID { get; set; }
        public Guid UserID { get; set; }
        public Guid? SkillID { get; set; }
        public Guid? RoadmapItemID { get; set; }
        public int? AttemptNumber { get; set; }
        public decimal? ScorePercentage { get; set; }
        public bool PerfectScore { get; set; }
        public DateTime? GeneratedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public User User { get; set; } = null!;
        public Skill? Skill { get; set; }
        public RoadmapItem? RoadmapItem { get; set; }
        public ICollection<AssessmentQuestion> Questions { get; set; } = new List<AssessmentQuestion>();
    }
}
