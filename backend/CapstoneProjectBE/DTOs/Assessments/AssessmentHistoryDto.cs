namespace CapstoneProjectBE.DTOs.Assessments
{
    public class AssessmentHistoryDto
    {
        public Guid SkillAssessmentID { get; set; }
        public Guid SkillID { get; set; }
        public Guid? RoadmapItemID { get; set; }
        public int AttemptNumber { get; set; }
        public decimal? ScorePercentage { get; set; }
        public bool PerfectScore { get; set; }
        public DateTime GeneratedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
