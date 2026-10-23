namespace CapstoneProjectBE.Models
{
    public class AssessmentQuestion
    {
        public Guid QuestionID { get; set; }
        public Guid SkillAssessmentID { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string[] Choices { get; set; } = Array.Empty<string>();
        public string CorrectAnswer { get; set; } = string.Empty;
        public int QuestionOrder { get; set; }
        public SkillAssessment SkillAssessment { get; set; } = null!;
        public ICollection<AssessmentResponse> Responses { get; set; } = new List<AssessmentResponse>();
    }
}
