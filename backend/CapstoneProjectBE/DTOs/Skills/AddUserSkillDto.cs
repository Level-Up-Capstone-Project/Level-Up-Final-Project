using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Skills
{
    public class AddUserSkillDto
    {
        [Required]
        public Guid? SkillID { get; set; }
    }
}
