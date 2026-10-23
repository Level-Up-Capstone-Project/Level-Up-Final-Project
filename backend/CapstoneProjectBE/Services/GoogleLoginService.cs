using System.Security.Claims;
using System.Security.Cryptography;
using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.Auth;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Services;

public sealed record GoogleLoginResult(LoginResponse Login, bool IsNewUser, string Name);

public sealed class GoogleLoginService(AppDbContext db, IPasswordHasher<User> hasher, IJwtTokenService jwt)
{
    public async Task<GoogleLoginResult> SignInAsync(ClaimsPrincipal principal, bool allowCreate, CancellationToken ct)
    {
        var subject = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = principal.FindFirstValue(ClaimTypes.Email)?.Trim().ToLowerInvariant();
        var verified = principal.FindFirstValue("google:email_verified");
        if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(email) || verified != "true")
            throw new InvalidDataException("Google must provide a verified email address to sign in.");
        var linked = await db.ExternalLogins.Include(login => login.User).ThenInclude(user => user.UserProfile)
            .SingleOrDefaultAsync(login => login.Provider == "google" && login.Subject == subject, ct);
        User user;
        var isNew = linked is null;
        if (linked is not null)
        {
            user = linked.User;
            if (!user.IsActive) throw new InvalidDataException("This account is inactive. Contact support.");
        }
        else
        {
            if (!allowCreate)
                throw new InvalidDataException("No Google account is linked to this email. Use Sign Up with Google first.");
            // Never silently link accounts using an email supplied by an external provider.
            if (await db.Users.AnyAsync(existing => existing.Email == email, ct))
                throw new InvalidDataException("An account with this email already exists. Please sign in with your email and password.");
            user = new User {UserID = Guid.NewGuid(), Email = email, CreatedAt = DateTime.UtcNow, IsActive = true};
            user.PasswordHash = hasher.HashPassword(user, Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)));
            db.ExternalLogins.Add(new ExternalLogin {Provider = "google", Subject = subject, User = user, UserID = user.UserID});
            await db.SaveChangesAsync(ct);
        }
        return new GoogleLoginResult(new LoginResponse
        {
            Token = jwt.GenerateToken(user),
            User = new AuthUserDto {UserID = user.UserID, Email = user.Email, IsActive = true},
            ProfileCompleted = user.UserProfile?.ProfileCompleted ?? false,
        }, isNew, principal.FindFirstValue(ClaimTypes.Name) ?? "");
    }
}
