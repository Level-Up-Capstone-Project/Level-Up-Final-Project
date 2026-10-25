namespace CapstoneProjectBE.Models
{
    /// <summary>
    /// Represents the persisted Assessment Response domain entity.
    /// </summary>
    public class AssessmentResponse
    {
        public Guid ResponseID { get; set; }
        public Guid QuestionID { get; set; }
        public string? UserAnswer { get; set; }
        public bool? IsCorrect { get; set; }
        public AssessmentQuestion Question { get; set; } = null!;
    }
}
