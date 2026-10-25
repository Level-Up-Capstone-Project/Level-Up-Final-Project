using CapstoneProjectBE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.RateLimiting;

namespace CapstoneProjectBE.Controllers;

/// <summary>
/// Handles HTTP requests for Mira operations.
/// </summary>
[Authorize]
[ApiController]
[Route("api/mira")]
public sealed class MiraController(N8nAnalysisService workflow) : ControllerBase
{
    /// <summary>
    /// Carries Chat Request request or response values used by the controller.
    /// </summary>
    public sealed record ChatRequest(string? Message);

    [EnableRateLimiting("ai-workflow")]
    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request, CancellationToken ct)
    {
        var message = request.Message?.Trim();
        if (string.IsNullOrEmpty(message) || message.Length > 4000) return BadRequest(new {message = "Enter a message of up to 4,000 characters."});
        try
        {
            var result = await workflow.ChatAsync(message, ct);
            if (result.ValueKind != JsonValueKind.Object || !result.TryGetProperty("success", out var ok) || ok.ValueKind != JsonValueKind.True ||
                !result.TryGetProperty("message", out var reply) || reply.ValueKind != JsonValueKind.Object ||
                !reply.TryGetProperty("content", out var content) || content.ValueKind != JsonValueKind.String ||
                string.IsNullOrWhiteSpace(content.GetString()) || content.GetString()!.Length > 20000)
                throw new N8nAnalysisException("MIRA returned an invalid response.");
            return Ok(new {message = content.GetString()});
        }
        catch (N8nAnalysisException error) { return Problem(title: error.Message, statusCode: error.StatusCode); }
    }
}
