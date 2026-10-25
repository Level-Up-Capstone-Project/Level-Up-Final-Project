using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Auth
{
    /// <summary>
    /// Carries Resend Verification Request data between API clients and application services.
    /// </summary>
    public class ResendVerificationRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; } = string.Empty;
    }
}
