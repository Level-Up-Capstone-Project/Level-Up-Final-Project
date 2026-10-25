using System.Security.Claims;
using CapstoneProjectBE.Data;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Xunit;

/// <summary>
/// Verifies Google Login behavior.
/// </summary>
public class GoogleLoginTests
{
    private static AppDbContext Database() => new(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
    private static ClaimsPrincipal Identity(string subject = "google-subject", bool verified = true) => new(new ClaimsIdentity([
        new Claim(ClaimTypes.NameIdentifier, subject), new Claim(ClaimTypes.Email, "person@gmail.com"),
        new Claim(ClaimTypes.Name, "Test Person"), new Claim("google:email_verified", verified ? "true" : "false"),
    ], "Google"));
    private static GoogleLoginService Service(AppDbContext db) => new(db, new PasswordHasher<User>(), new Jwt());

    [Fact]
    public async Task CreatesVerifiedGoogleAccountAndReusesStableSubject()
    {
        using var db = Database();
        var first = await Service(db).SignInAsync(Identity(), true, default);
        var second = await Service(db).SignInAsync(Identity(), false, default);
        Assert.True(first.IsNewUser);
        Assert.False(second.IsNewUser);
        Assert.Equal(first.Login.User.UserID, second.Login.User.UserID);
        Assert.True(first.Login.User.IsActive);
        Assert.Single(db.Users);
        Assert.Equal("google-subject", db.ExternalLogins.Single().Subject);
    }

    [Fact]
    public async Task DoesNotSilentlyLinkAnExistingPasswordAccount()
    {
        using var db = Database();
        db.Users.Add(new User {UserID = Guid.NewGuid(), Email = "person@gmail.com", PasswordHash = "existing", IsActive = true});
        await db.SaveChangesAsync();
        await Assert.ThrowsAsync<InvalidDataException>(() => Service(db).SignInAsync(Identity(), true, default));
        Assert.Empty(db.ExternalLogins);
    }

    [Fact]
    public async Task UnverifiedEmailCannotCreateAnAccount()
    {
        using var db = Database();
        await Assert.ThrowsAsync<InvalidDataException>(() => Service(db).SignInAsync(Identity(verified: false), true, default));
        Assert.Empty(db.Users);
    }

    [Fact]
    public async Task InactiveAccountCannotSignIn()
    {
        using var db = Database();
        await Service(db).SignInAsync(Identity(), true, default);
        db.Users.Single().IsActive = false;
        await db.SaveChangesAsync();
        await Assert.ThrowsAsync<InvalidDataException>(() => Service(db).SignInAsync(Identity(), false, default));
    }

    [Fact]
    public async Task SignInDoesNotCreateAnUnknownGoogleAccount()
    {
        using var db = Database();
        var error = await Assert.ThrowsAsync<InvalidDataException>(() => Service(db).SignInAsync(Identity(), false, default));
        Assert.Contains("Sign Up", error.Message);
        Assert.Empty(db.Users);
        Assert.Empty(db.ExternalLogins);
    }

    /// <summary>
    /// Verifies Jwt behavior.
    /// </summary>
    private sealed class Jwt : IJwtTokenService { public string GenerateToken(User user) => "application-token"; }
}
