using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Profile
{
    public class UpdateTargetCareerDto
    {
        [Required]
        public Guid? CareerID { get; set; }
    }
}
