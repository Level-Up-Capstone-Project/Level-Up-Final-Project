namespace CapstoneProjectBE.ExternalServices.Email
{
    public interface IEmailService
    {
        Task SendVerificationCodeAsync(string recipientEmail, string verificationCode, CancellationToken cancellationToken);
        Task SendPasswordResetCodeAsync(string recipientEmail, string verificationCode, CancellationToken cancellationToken);
        Task SendPasswordChangedAsync(string recipientEmail, CancellationToken cancellationToken);
    }
}
