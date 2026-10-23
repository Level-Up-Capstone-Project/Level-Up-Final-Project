namespace CapstoneProjectBE.DTOs.Assessments
{
    public class AssessmentDto
    {
        public Guid SkillAssessmentID { get; set; }
        public Guid SkillID { get; set; }
        public Guid RoadmapItemID { get; set; }
        public int AttemptNumber { get; set; }
        public DateTime GeneratedAt { get; set; }
        public IReadOnlyList<AssessmentQuestionDto> Questions { get; set; } = Array.Empty<AssessmentQuestionDto>();
    }

    public class AssessmentQuestionDto
    {
        public Guid QuestionID { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public IReadOnlyList<string> Choices { get; set; } = Array.Empty<string>();
        public int QuestionOrder { get; set; }
    }
}
