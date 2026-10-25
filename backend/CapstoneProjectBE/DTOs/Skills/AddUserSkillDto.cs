using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Skills
{
    /// <summary>
    /// Carries Add User Skill data between API clients and application services.
    /// </summary>
    public class AddUserSkillDto
    {
        [Required]
        public Guid? SkillID { get; set; }
    }
}
