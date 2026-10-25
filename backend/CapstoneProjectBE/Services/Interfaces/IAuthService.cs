using CapstoneProjectBE.DTOs.Auth;

namespace CapstoneProjectBE.Services.Interfaces
{
    /// <summary>
    /// Defines the operations provided by the Auth service abstraction.
    /// </summary>
    public interface IAuthService
    {
        Task<RegisterResponse?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
        Task<VerifyEmailResponse?> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken);
        Task ResendVerificationAsync(ResendVerificationRequest request, CancellationToken cancellationToken);
        Task RequestPasswordResetAsync(PasswordResetCodeRequest request, CancellationToken cancellationToken);
        Task<bool> ResetPasswordAsync(CompletePasswordResetRequest request, CancellationToken cancellationToken);
        Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
        Task<AuthUserDto?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken);
    }
}
