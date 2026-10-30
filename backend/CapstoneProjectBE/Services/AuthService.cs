using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.Auth;
using CapstoneProjectBE.ExternalServices.Email;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Services
{
    /// <summary>
    /// Coordinates Auth service application behavior.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IPasswordHasher<PasswordReset> _resetCodeHasher;
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly IEmailService _emailService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AppDbContext context, IPasswordHasher<User> passwordHasher, IPasswordHasher<PasswordReset> resetCodeHasher, IVerificationCodeService verificationCodeService, IEmailService emailService, IJwtTokenService jwtTokenService, ILogger<AuthService> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _resetCodeHasher = resetCodeHasher;
            _verificationCodeService = verificationCodeService;
            _emailService = emailService;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        public async Task<RegisterResponse?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
        {
            string email = request.Email.Trim().ToLowerInvariant();
            bool emailExists = await _context.Users.AnyAsync(user => user.Email == email, cancellationToken);

            if (emailExists)
            {
                return null;
            }

            DateTime now = DateTime.UtcNow;

            var user = new User
            {
                UserID = Guid.NewGuid(),
                Email = email,
                CreatedAt = now,
                IsActive = false
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            var nameParts = (request.Name ?? string.Empty).Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var profile = new UserProfile
            {
                ProfileID = Guid.NewGuid(),
                UserID = user.UserID,
                User = user,
                FName = nameParts.FirstOrDefault() ?? string.Empty,
                LName = nameParts.Length > 1 ? nameParts[1] : string.Empty,
                Qualification = string.Empty,
                FieldOfStudy = string.Empty,
                Industry = NormalizeOptionalValue(request.Industry),
                Phone = NormalizeOptionalValue(request.Phone),
                TargetRoles = NormalizeTargetRoles(request.TargetRoles),
                WorkAreasJson = System.Text.Json.JsonSerializer.Serialize(request.WorkAreas ?? new())
            };
            user.UserProfile = profile;
            string verificationCode = _verificationCodeService.GenerateCode();

            var emailVerification = new EmailVerification
            {
                EmailVerificationID = Guid.NewGuid(),
                UserID = user.UserID,
                User = user,
                ExpiresAt = now.AddMinutes(15),
                LastSentAt = now,
                FailedAttempts = 0
            };

            emailVerification.CodeHash = _verificationCodeService.HashVerificationCode(emailVerification, verificationCode);
            user.EmailVerification = emailVerification;
            _context.Users.Add(user);

            await _context.SaveChangesAsync(cancellationToken);
            await _emailService.SendVerificationCodeAsync(email, verificationCode, cancellationToken);

            return new RegisterResponse
            {
                UserID = user.UserID,
                Email = user.Email,
                IsActive = user.IsActive,
                Token = null
            };
        }

        public async Task RequestPasswordResetAsync(PasswordResetCodeRequest request, CancellationToken cancellationToken)
        {
            string email = request.Email.Trim().ToLowerInvariant();
            User? user = await _context.Users
                .Include(item => item.PasswordReset)
                .SingleOrDefaultAsync(item => item.Email == email && item.IsActive, cancellationToken);
            if (user is null) return;
            if (await _context.ExternalLogins.AnyAsync(login => login.UserID == user.UserID, cancellationToken)) return;

            DateTime now = DateTime.UtcNow;
            PasswordReset? reset = user.PasswordReset;
            if (reset is not null && reset.LastSentAt > now.AddMinutes(-1)) return;

            string code = _verificationCodeService.GenerateCode();
            if (reset is null)
            {
                reset = new PasswordReset {PasswordResetID = Guid.NewGuid(), UserID = user.UserID, User = user};
                user.PasswordReset = reset;
                _context.PasswordResets.Add(reset);
            }
            reset.CodeHash = _resetCodeHasher.HashPassword(reset, code);
            reset.ExpiresAt = now.AddMinutes(15);
            reset.LastSentAt = now;
            reset.FailedAttempts = 0;
            await _context.SaveChangesAsync(cancellationToken);
            try
            {
                await _emailService.SendPasswordResetCodeAsync(email, code, cancellationToken);
            }
            catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
            {
                reset.LastSentAt = now.AddMinutes(-1);
                await _context.SaveChangesAsync(CancellationToken.None);
                _logger.LogError(exception, "Password reset email delivery failed for user {UserId}.", user.UserID);
            }
        }

        public async Task<bool> ResetPasswordAsync(CompletePasswordResetRequest request, CancellationToken cancellationToken)
        {
            var verifiedReset = await GetVerifiedPasswordResetAsync(
                request.Email,
                request.VerificationCode,
                cancellationToken);
            if (verifiedReset is null) return false;

            var (user, reset) = verifiedReset.Value;

            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            _context.PasswordResets.Remove(reset);
            await _context.SaveChangesAsync(cancellationToken);
            try
            {
                await _emailService.SendPasswordChangedAsync(user.Email, cancellationToken);
            }
            catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
            {
                _logger.LogWarning(exception, "Password changed notification delivery failed for user {UserId}.", user.UserID);
            }
            return true;
        }

        public async Task<bool> VerifyPasswordResetCodeAsync(VerifyPasswordResetCodeRequest request, CancellationToken cancellationToken) =>
            await GetVerifiedPasswordResetAsync(request.Email, request.VerificationCode, cancellationToken) is not null;

        public async Task<VerifyEmailResponse?> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken)
        {
            string email = request.Email.Trim().ToLowerInvariant();

            User? user = await _context.Users.Include(user => user.EmailVerification).SingleOrDefaultAsync(user => user.Email == email, cancellationToken);

            if (user is null || user.IsActive || user.EmailVerification is null)
            {
                return null;
            }

            EmailVerification verification = user.EmailVerification;

            if (verification.ExpiresAt <= DateTime.UtcNow || verification.FailedAttempts >= 5)
            {
                return null;
            }

            bool codeValid = _verificationCodeService.VerifyVerificationCode(verification, verification.CodeHash, request.VerificationCode);

            if (!codeValid)
            {
                verification.FailedAttempts++;

                await _context.SaveChangesAsync(cancellationToken);
                return null;
            }

            user.IsActive = true;

            _context.EmailVerifications.Remove(verification);

            await _context.SaveChangesAsync(cancellationToken);

            string token = _jwtTokenService.GenerateToken(user);

            return new VerifyEmailResponse
            {
                Message = "Successful email verification.",
                Token = token
            };
        }

        public async Task ResendVerificationAsync(ResendVerificationRequest request, CancellationToken cancellationToken)
        {
            string email = request.Email.Trim().ToLowerInvariant();

            User? user = await _context.Users.Include(user => user.EmailVerification).SingleOrDefaultAsync(user => user.Email == email, cancellationToken);

            if (user is null || user.IsActive)
            {
                return;
            }

            DateTime now = DateTime.UtcNow;

            if (user.EmailVerification is not null && user.EmailVerification.LastSentAt > now.AddMinutes(-1))
            {
                return;
            }

            string verificationCode = _verificationCodeService.GenerateCode();

            EmailVerification verification;

            if (user.EmailVerification is null)
            {
                verification = new EmailVerification
                {
                    EmailVerificationID = Guid.NewGuid(),
                    UserID = user.UserID,
                    User = user
                };

                user.EmailVerification = verification;
                _context.EmailVerifications.Add(verification);
            }

            else
            {
                verification = user.EmailVerification;
            }

            verification.CodeHash = _verificationCodeService.HashVerificationCode(verification, verificationCode);

            verification.ExpiresAt = now.AddMinutes(15);
            verification.LastSentAt = now;
            verification.FailedAttempts = 0;

            await _context.SaveChangesAsync(cancellationToken);
            await _emailService.SendVerificationCodeAsync(email, verificationCode, cancellationToken);
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            string email = request.Email.Trim().ToLowerInvariant();

            User? user = await _context.Users.Include(user => user.UserProfile).SingleOrDefaultAsync(user => user.Email == email, cancellationToken);

            if (user is null)
            {
                return LoginResult.InvalidCredentials();
            }

            PasswordVerificationResult passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if(passwordResult == PasswordVerificationResult.Failed)
            {
                return LoginResult.InvalidCredentials();
            }

            if (!user.IsActive)
            {
                return LoginResult.VerificationRequired();
            }

            if (passwordResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

                await _context.SaveChangesAsync(cancellationToken);
            }

            string token = _jwtTokenService.GenerateToken(user);

            return LoginResult.Success(new LoginResponse
            {
                Token = token,
                User = new AuthUserDto
                {
                    UserID = user.UserID,
                    Email = user.Email,
                    IsActive = user.IsActive
                },
                ProfileCompleted = user.UserProfile?.ProfileCompleted ?? false
            });
        }

        public async Task<AuthUserDto?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Users.AsNoTracking().Where(user => user.UserID == userId && user.IsActive).Select(user => new AuthUserDto
            {
                UserID = user.UserID,
                Email = user.Email,
                IsActive = user.IsActive
            }).SingleOrDefaultAsync(cancellationToken);
        }

        private static string? NormalizeOptionalValue(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static string[] NormalizeTargetRoles(IEnumerable<string>? values) =>
            (values ?? Array.Empty<string>())
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        private async Task<(User User, PasswordReset Reset)?> GetVerifiedPasswordResetAsync(
            string emailAddress,
            string verificationCode,
            CancellationToken cancellationToken)
        {
            string email = emailAddress.Trim().ToLowerInvariant();
            User? user = await _context.Users
                .Include(item => item.PasswordReset)
                .SingleOrDefaultAsync(item => item.Email == email && item.IsActive, cancellationToken);
            PasswordReset? reset = user?.PasswordReset;
            if (user is null || reset is null || reset.ExpiresAt <= DateTime.UtcNow || reset.FailedAttempts >= 5)
                return null;

            PasswordVerificationResult verification = _resetCodeHasher.VerifyHashedPassword(
                reset,
                reset.CodeHash,
                verificationCode);
            if (verification == PasswordVerificationResult.Failed)
            {
                reset.FailedAttempts++;
                await _context.SaveChangesAsync(cancellationToken);
                return null;
            }

            return (user, reset);
        }
    }
}
