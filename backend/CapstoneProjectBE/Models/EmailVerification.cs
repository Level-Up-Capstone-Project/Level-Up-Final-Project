namespace CapstoneProjectBE.Models
{
    /// <summary>
    /// Represents the persisted Email Verification domain entity.
    /// </summary>
    public class EmailVerification
    {
        public Guid EmailVerificationID { get; set; }
        public Guid UserID { get; set; }
        public string CodeHash { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime LastSentAt { get; set; }
        public int FailedAttempts { get; set; }
        public User User { get; set; } = null!;
    }
}
