using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Assessments
{
    public class StartAssessmentDto
    {
        [Required]
        public Guid? SkillID { get; set; }

        [Required]
        public Guid? RoadmapItemID { get; set; }
    }
}
