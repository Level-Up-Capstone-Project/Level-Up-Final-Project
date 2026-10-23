namespace CapstoneProjectBE.DTOs.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public AuthUserDto User { get; set; } = new();
        public bool ProfileCompleted { get; set; }
    }
}
