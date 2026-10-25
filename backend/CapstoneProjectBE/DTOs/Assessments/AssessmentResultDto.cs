namespace CapstoneProjectBE.DTOs.Assessments
{
    /// <summary>
    /// Carries Assessment Result data between API clients and application services.
    /// </summary>
    public class AssessmentResultDto
    {
        public Guid SkillAssessmentID { get; set; }
        public int AttemptNumber { get; set; }
        public decimal ScorePercentage { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public bool PerfectScore { get; set; }
        public DateTime CompletedAt { get; set; }
        public AssessmentSkillResultDto Skill { get; set; } = new();
        public IReadOnlyList<AssessmentAnswerReviewDto> Answers { get; set; } = Array.Empty<AssessmentAnswerReviewDto>();
    }

    /// <summary>
    /// Carries Assessment Answer Review data between API clients and application services.
    /// </summary>
    public class AssessmentAnswerReviewDto
    {
        public Guid QuestionID { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string UserAnswer { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    /// <summary>
    /// Carries Assessment Skill Result data between API clients and application services.
    /// </summary>
    public class AssessmentSkillResultDto
    {
        public Guid SkillID { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsKnowledgeVerified { get; set; }
    }
}
