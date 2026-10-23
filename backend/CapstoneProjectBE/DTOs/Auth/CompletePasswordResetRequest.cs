using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Auth
{
    public class CompletePasswordResetRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^[0-9]{6}$")]
        public string VerificationCode { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [MaxLength(128)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
