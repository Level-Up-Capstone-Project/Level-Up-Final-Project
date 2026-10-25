namespace CapstoneProjectBE.ExternalServices.Email
{
    /// <summary>
    /// Defines the contract for the Email Service integration.
    /// </summary>
    public interface IEmailService
    {
        Task SendVerificationCodeAsync(string recipientEmail, string verificationCode, CancellationToken cancellationToken);
        Task SendPasswordResetCodeAsync(string recipientEmail, string verificationCode, CancellationToken cancellationToken);
        Task SendPasswordChangedAsync(string recipientEmail, CancellationToken cancellationToken);
    }
}
