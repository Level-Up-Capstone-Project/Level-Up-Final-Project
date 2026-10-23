using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.Auth;
using CapstoneProjectBE.DTOs.Profile;
using CapstoneProjectBE.ExternalServices.Email;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CapstoneProjectBE.Tests;

public sealed class AuthRecoveryAndPreferencesTests
{
    [Fact]
    public async Task RegistrationPersistsSignupPreferences()
    {
        await using var context = CreateContext();
        var email = new CapturingEmailService();
        var hasher = new PasswordHasher<User>();
        var service = CreateAuthService(context, email, hasher);

        var response = await service.RegisterAsync(new RegisterRequest
        {
            Email = "person@example.com",
            Password = "Password123!",
            Name = "Ada Lovelace",
            Industry = "Technology",
            Phone = "+27123456789",
            TargetRoles = ["Software Engineer"],
            WorkAreas = [new WorkAreaDto
            {
                PlaceId = "province:ZA:Gauteng", Label = "Gauteng", CountryCode = "ZA",
                Kind = "region", Names = ["Gauteng", "South Africa"]
            }]
        }, default);

        Assert.NotNull(response);
        var profile = await context.UserProfiles.SingleAsync();
        Assert.Equal("Ada", profile.FName);
        Assert.Equal("Lovelace", profile.LName);
        Assert.Equal("Technology", profile.Industry);
        Assert.Equal("+27123456789", profile.Phone);
        Assert.Equal(["Software Engineer"], profile.TargetRoles);
        Assert.Contains("Gauteng", profile.WorkAreasJson);
        var loaded = await new ProfileService(context).GetProfileAsync(response.UserID, default);
        Assert.Equal("Technology", loaded?.Industry);
        Assert.Equal("Gauteng", Assert.Single(loaded!.WorkAreas).Label);
    }

    [Fact]
    public async Task PasswordResetChangesPasswordAndConsumesCode()
    {
        await using var context = CreateContext();
        var email = new CapturingEmailService();
        var hasher = new PasswordHasher<User>();
        var user = new User
        {
            UserID = Guid.NewGuid(), Email = "person@example.com", CreatedAt = DateTime.UtcNow, IsActive = true
        };
        user.PasswordHash = hasher.HashPassword(user, "OldPassword123!");
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var service = CreateAuthService(context, email, hasher);

        await service.RequestPasswordResetAsync(new PasswordResetCodeRequest {Email = user.Email}, default);
        Assert.Matches("^[0-9]{6}$", email.ResetCode);
        var request = new CompletePasswordResetRequest
        {
            Email = user.Email, VerificationCode = email.ResetCode, NewPassword = "NewPassword123!"
        };

        Assert.True(await service.ResetPasswordAsync(request, default));
        Assert.False(await service.ResetPasswordAsync(request, default));
        var updated = await context.Users.SingleAsync();
        Assert.Equal(PasswordVerificationResult.Success, hasher.VerifyHashedPassword(updated, updated.PasswordHash, request.NewPassword));
        Assert.Empty(context.PasswordResets);
        Assert.True(email.PasswordChangedNotificationSent);
    }

    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static AuthService CreateAuthService(AppDbContext context, CapturingEmailService email, IPasswordHasher<User> hasher) =>
        new(context, hasher, new PasswordHasher<PasswordReset>(),
            new VerificationCodeService(new PasswordHasher<EmailVerification>()), email, new StubJwtService(),
            NullLogger<AuthService>.Instance);

    private sealed class CapturingEmailService : IEmailService
    {
        public string ResetCode { get; private set; } = string.Empty;
        public bool PasswordChangedNotificationSent { get; private set; }

        public Task SendVerificationCodeAsync(string recipientEmail, string verificationCode, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task SendPasswordResetCodeAsync(string recipientEmail, string verificationCode, CancellationToken cancellationToken)
        {
            ResetCode = verificationCode;
            return Task.CompletedTask;
        }

        public Task SendPasswordChangedAsync(string recipientEmail, CancellationToken cancellationToken)
        {
            PasswordChangedNotificationSent = true;
            return Task.CompletedTask;
        }
    }

    private sealed class StubJwtService : IJwtTokenService
    {
        public string GenerateToken(User user) => "test-token";
    }
}
