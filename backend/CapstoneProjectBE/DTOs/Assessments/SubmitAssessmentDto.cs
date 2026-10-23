using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Assessments
{
    public class SubmitAssessmentDto
    {
        [Required]
        [MinLength(1)]
        public List<AssessmentResponseRequestDto> Responses { get; set; } = new();
    }

    public class AssessmentResponseRequestDto
    {
        [Required]
        public Guid? QuestionID { get; set; }

        [Required]
        [StringLength(2000)]
        public string UserAnswer { get; set; } = string.Empty;
    }
}
