using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Profile
{
    /// <summary>
    /// Carries Update Profile Preferences data between API clients and application services.
    /// </summary>
    public class UpdateProfilePreferencesDto
    {
        [StringLength(100)]
        public string? Industry { get; set; }

        [StringLength(30)]
        public string? Phone { get; set; }

        [MaxLength(20)]
        public List<string> TargetRoles { get; set; } = new();

        [MaxLength(10)]
        public List<WorkAreaDto> WorkAreas { get; set; } = new();
    }
}
