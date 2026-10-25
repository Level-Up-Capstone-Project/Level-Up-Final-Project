using System.Text.Json;
using CapstoneProjectBE.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;

namespace CapstoneProjectBE.Controllers;

/// <summary>
/// Handles HTTP requests for Google Auth operations.
/// </summary>
[ApiController, AllowAnonymous, Route("api/auth/google")]
public class GoogleAuthController(IConfiguration config, GoogleLoginService login) : ControllerBase
{
    public const string ExternalCookie = "GoogleExternal";
    private const string IntentKey = "levelup:google-intent";
    private const string ReturnOriginKey = "levelup:google-return-origin";
    private bool Configured => !string.IsNullOrWhiteSpace(config["Authentication:Google:ClientId"]) &&
                               !string.IsNullOrWhiteSpace(config["Authentication:Google:ClientSecret"]);

    [HttpGet("config")]
    public IActionResult Configuration() => Ok(new {configured = Configured});

    [EnableRateLimiting("authentication")]
    [HttpGet("start")]
    public IActionResult Start([FromQuery] string intent = "signin", [FromQuery] string? returnOrigin = null)
    {
        var allowedOrigins = (config["Frontend:Origins"] ?? config["Frontend:Origin"] ?? "http://localhost:3000").Split([';', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var origin = returnOrigin ?? allowedOrigins[0];
        if (!allowedOrigins.Contains(origin, StringComparer.Ordinal))
            return Popup(new {type = "levelup-google", error = "This frontend is not allowed to use Google sign-in."});
        if (!Configured) return Popup(new {type = "levelup-google", error = "Google sign-in needs backend OAuth credentials before it can be used."}, origin);
        if (intent is not ("signin" or "signup"))
            return Popup(new {type = "levelup-google", error = "Choose Google sign in or sign up and try again."}, origin);
        var properties = new AuthenticationProperties { RedirectUri = "/api/auth/google/complete", IsPersistent = false };
        properties.Items[IntentKey] = intent;
        properties.Items[ReturnOriginKey] = origin;
        return Challenge(properties, "Google");
    }

    [HttpGet("complete")]
    public async Task<IActionResult> Complete(CancellationToken ct)
    {
        var external = await HttpContext.AuthenticateAsync(ExternalCookie);
        await HttpContext.SignOutAsync(ExternalCookie);
        var returnOrigin = external.Properties?.Items.TryGetValue(ReturnOriginKey, out var origin) == true ? origin : null;
        if (!external.Succeeded || external.Principal is null)
            return Popup(new {type = "levelup-google", error = "Google authentication did not complete. Please retry."}, returnOrigin);
        try
        {
            var allowCreate = external.Properties?.Items.TryGetValue(IntentKey, out var intent) == true && intent == "signup";
            var result = await login.SignInAsync(external.Principal, allowCreate, ct);
            return Popup(new {type = "levelup-google", result}, returnOrigin);
        }
        catch (InvalidDataException error) { return Popup(new {type = "levelup-google", error = error.Message}, returnOrigin); }
        catch (DbUpdateException) { return Popup(new {type = "levelup-google", error = "Could not create the Google account. Try signing in again."}, returnOrigin); }
        catch (Npgsql.PostgresException error) when (error.SqlState == "42P01")
        { return Popup(new {type = "levelup-google", error = "Google sign-in database setup is incomplete. Apply the EF database migration."}, returnOrigin); }
    }

    [HttpGet("error")]
    public IActionResult Error() => Popup(new {type = "levelup-google", error = "Google sign-in was cancelled or failed. Please retry."});

    private ContentResult Popup(object payload, string? returnOrigin = null)
    {
        var allowedOrigins = (config["Frontend:Origins"] ?? config["Frontend:Origin"] ?? "http://localhost:3000").Split([';', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var origin = returnOrigin is not null && allowedOrigins.Contains(returnOrigin, StringComparer.Ordinal) ? returnOrigin : allowedOrigins[0];
        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri) || uri.GetLeftPart(UriPartial.Authority) != origin.TrimEnd('/'))
            throw new InvalidOperationException("Frontend:Origin must be a single frontend origin.");
        var nonce = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(24));
        Response.Headers.CacheControl = "no-store";
        Response.Headers["Referrer-Policy"] = "no-referrer";
        Response.Headers["Content-Security-Policy"] = $"default-src 'none'; script-src 'nonce-{nonce}'; base-uri 'none'; frame-ancestors 'none'";
        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        return Content($"<!doctype html><html><head><title>Google sign-in</title></head><body>Returning to Level Up…<script nonce=\"{nonce}\">if(window.opener){{window.opener.postMessage({json},{JsonSerializer.Serialize(origin.TrimEnd('/'))});window.close();}}</script></body></html>", "text/html");
    }
}
