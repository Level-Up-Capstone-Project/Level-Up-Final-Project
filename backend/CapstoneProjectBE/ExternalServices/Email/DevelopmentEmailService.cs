namespace CapstoneProjectBE.ExternalServices.Email
{
    /// <summary>
    /// Provides Development Email Service integration behavior.
    /// </summary>
    public class DevelopmentEmailService : IEmailService
    {
        private readonly ILogger<DevelopmentEmailService> _logger;
        private readonly IHostEnvironment _environment;

        public DevelopmentEmailService(ILogger<DevelopmentEmailService> logger, IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public Task SendVerificationCodeAsync(string recipientEmail, string verificationCode, CancellationToken cancellationToken)
        {
            if(!_environment.IsDevelopment())
            {
                throw new InvalidOperationException("The dev email service cannot be used outside development");
            }
            _logger.LogInformation("DEVELOPMENT ONLY - Verification code for {RecipientEmail}: {VerificationCode}", recipientEmail, verificationCode);
            return Task.CompletedTask;
        }

        public Task SendPasswordResetCodeAsync(string recipientEmail, string verificationCode, CancellationToken cancellationToken)
        {
            if (!_environment.IsDevelopment())
            {
                throw new InvalidOperationException("The dev email service cannot be used outside development");
            }
            _logger.LogInformation("DEVELOPMENT ONLY - Password reset code for {RecipientEmail}: {VerificationCode}", recipientEmail, verificationCode);
            return Task.CompletedTask;
        }

        public Task SendPasswordChangedAsync(string recipientEmail, CancellationToken cancellationToken)
        {
            if (!_environment.IsDevelopment())
            {
                throw new InvalidOperationException("The dev email service cannot be used outside development");
            }
            _logger.LogInformation("DEVELOPMENT ONLY - Password changed notification for {RecipientEmail}", recipientEmail);
            return Task.CompletedTask;
        }
    }
}
