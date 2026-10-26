namespace CapstoneProjectBE.DTOs.Assessments
{
    /// <summary>
    /// Carries Assessment data between API clients and application services.
    /// </summary>
    public class AssessmentDto
    {
        public Guid SkillAssessmentID { get; set; }
        public Guid SkillID { get; set; }
        public Guid RoadmapItemID { get; set; }
        public int AttemptNumber { get; set; }
        public DateTime GeneratedAt { get; set; }
        public IReadOnlyList<AssessmentQuestionDto> Questions { get; set; } = Array.Empty<AssessmentQuestionDto>();
    }

    /// <summary>
    /// Carries Assessment Question data between API clients and application services.
    /// </summary>
    public class AssessmentQuestionDto
    {
        public Guid QuestionID { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = "MultipleChoice";
        public IReadOnlyList<string> Choices { get; set; } = Array.Empty<string>();
        public int QuestionOrder { get; set; }
    }
}
