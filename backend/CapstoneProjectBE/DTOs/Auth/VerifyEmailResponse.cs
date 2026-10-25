namespace CapstoneProjectBE.DTOs.Auth
{
    /// <summary>
    /// Carries Verify Email Response data between API clients and application services.
    /// </summary>
    public class VerifyEmailResponse
    {
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
