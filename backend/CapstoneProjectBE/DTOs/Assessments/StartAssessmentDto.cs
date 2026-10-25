using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Assessments
{
    /// <summary>
    /// Carries Start Assessment data between API clients and application services.
    /// </summary>
    public class StartAssessmentDto
    {
        [Required]
        public Guid? SkillID { get; set; }

        [Required]
        public Guid? RoadmapItemID { get; set; }
    }
}
