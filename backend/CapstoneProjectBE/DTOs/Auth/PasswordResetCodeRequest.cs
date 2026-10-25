using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Auth
{
    /// <summary>
    /// Carries Password Reset Code Request data between API clients and application services.
    /// </summary>
    public class PasswordResetCodeRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; } = string.Empty;
    }
}
