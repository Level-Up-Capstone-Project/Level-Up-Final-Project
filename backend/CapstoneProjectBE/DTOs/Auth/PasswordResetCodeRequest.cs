using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Auth
{
    public class PasswordResetCodeRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; } = string.Empty;
    }
}
