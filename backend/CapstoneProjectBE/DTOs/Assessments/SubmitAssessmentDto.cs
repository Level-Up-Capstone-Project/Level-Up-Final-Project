using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Assessments
{
    /// <summary>
    /// Carries Submit Assessment data between API clients and application services.
    /// </summary>
    public class SubmitAssessmentDto
    {
        [Required]
        [MinLength(1)]
        public List<AssessmentResponseRequestDto> Responses { get; set; } = new();
    }

    /// <summary>
    /// Carries Assessment Response Request data between API clients and application services.
    /// </summary>
    public class AssessmentResponseRequestDto
    {
        [Required]
        public Guid? QuestionID { get; set; }

        [Required]
        [StringLength(2000)]
        public string UserAnswer { get; set; } = string.Empty;
    }
}
