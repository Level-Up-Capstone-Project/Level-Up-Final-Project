using CapstoneProjectBE.DTOs.Profile;
using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [MaxLength(128)]
        public string Password { get; set; } = string.Empty;    

        [MaxLength(200)]
        public string? Name { get; set; }

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
