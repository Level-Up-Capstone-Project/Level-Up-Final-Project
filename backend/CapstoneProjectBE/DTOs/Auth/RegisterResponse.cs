namespace CapstoneProjectBE.DTOs.Auth
{
    /// <summary>
    /// Carries Register Response data between API clients and application services.
    /// </summary>
    public class RegisterResponse
    {
        public Guid UserID { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Token { get; set; }
    }
}
