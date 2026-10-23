using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Auth
{
    public class ResendVerificationRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; } = string.Empty;
    }
}
