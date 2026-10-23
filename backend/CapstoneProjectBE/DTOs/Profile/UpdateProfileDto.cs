using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Profile
{
    public class UpdateProfileDto
    {
        [Required]
        [StringLength (100)]
        public string FName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LName {  get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Qualification {  get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string FieldOfStudy {  get; set; } = string.Empty;

        [StringLength(100)]
        public string? ExperienceLevel { get; set; }

        [StringLength(1000)]
        public string? Interests { get; set; }

        [StringLength(100)]
        public string? Industry { get; set; }

        [StringLength(30)]
        public string? Phone { get; set; }

        [MaxLength(20)]
        public List<string>? TargetRoles { get; set; }

        [MaxLength(10)]
        public List<WorkAreaDto>? WorkAreas { get; set; }

        public Guid? TargetCareerID { get; set; }
    }
}
