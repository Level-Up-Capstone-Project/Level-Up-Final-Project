using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Auth
{
    public class VerifyEmailRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Verification code must contain 6 digits.")]
        public string VerificationCode { get; set; } = string.Empty;
    }
}
