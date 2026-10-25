namespace CapstoneProjectBE.DTOs.Auth
{
    /// <summary>
    /// Carries Auth User data between API clients and application services.
    /// </summary>
    public class AuthUserDto
    {
        public Guid UserID { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
