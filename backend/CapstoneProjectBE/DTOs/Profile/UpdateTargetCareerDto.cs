using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Profile
{
    /// <summary>
    /// Carries Update Target Career data between API clients and application services.
    /// </summary>
    public class UpdateTargetCareerDto
    {
        [Required]
        public Guid? CareerID { get; set; }
    }
}
