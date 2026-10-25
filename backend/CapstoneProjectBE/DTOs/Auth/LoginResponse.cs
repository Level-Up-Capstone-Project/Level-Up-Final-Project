namespace CapstoneProjectBE.DTOs.Auth
{
    /// <summary>
    /// Carries Login Response data between API clients and application services.
    /// </summary>
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public AuthUserDto User { get; set; } = new();
        public bool ProfileCompleted { get; set; }
    }
}
