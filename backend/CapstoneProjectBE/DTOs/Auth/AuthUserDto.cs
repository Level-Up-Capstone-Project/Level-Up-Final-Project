namespace CapstoneProjectBE.DTOs.Auth
{
    public class AuthUserDto
    {
        public Guid UserID { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
