namespace CapstoneProjectBE.Models
{
    public class AssessmentResponse
    {
        public Guid ResponseID { get; set; }
        public Guid QuestionID { get; set; }
        public string? UserAnswer { get; set; }
        public bool? IsCorrect { get; set; }
        public AssessmentQuestion Question { get; set; } = null!;
    }
}
