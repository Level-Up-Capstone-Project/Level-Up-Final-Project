namespace CapstoneProjectBE.Models
{
    /// <summary>
    /// Represents the persisted Password Reset domain entity.
    /// </summary>
    public class PasswordReset
    {
        public Guid PasswordResetID { get; set; }
        public Guid UserID { get; set; }
        public string CodeHash { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime LastSentAt { get; set; }
        public int FailedAttempts { get; set; }
        public User User { get; set; } = null!;
    }
}
