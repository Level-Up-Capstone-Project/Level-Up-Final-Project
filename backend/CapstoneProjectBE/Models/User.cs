namespace CapstoneProjectBE.Models
{
    public class User
    {
        public Guid UserID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public UserProfile? UserProfile { get; set; }
        public EmailVerification? EmailVerification { get; set; }
        public PasswordReset? PasswordReset { get; set; }
        public ICollection<CV> CVs { get; set; } = new List<CV>();
    }
}
