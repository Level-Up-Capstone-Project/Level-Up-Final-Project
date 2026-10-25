using System.Net;
using System.Text.Json;
using CapstoneProjectBE.ExternalServices.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CapstoneProjectBE.Tests;

/// <summary>
/// Verifies Bird Email Service behavior.
/// </summary>
public class BirdEmailServiceTests
{
    [Fact]
    public async Task PasswordRecoveryEmailIsTransactionalAndHasTextFallback()
    {
        var handler = new RecordingHandler();
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://eu1.platform.bird.com/") };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Bird:ApiKey"] = "bird-secret",
            ["Bird:FromEmail"] = "verify@mail.levelupnow.co.za",
            ["Bird:FromName"] = "LevelUp"
        }).Build();
        var service = new BirdEmailService(client, configuration, NullLogger<BirdEmailService>.Instance);

        await service.SendPasswordResetCodeAsync("person@example.com", "123456", default);

        Assert.NotNull(handler.Request);
        Assert.Equal(HttpMethod.Post, handler.Request.Method);
        Assert.Equal("Bearer", handler.Request.Headers.Authorization?.Scheme);
        Assert.Equal("bird-secret", handler.Request.Headers.Authorization?.Parameter);
        using JsonDocument payload = JsonDocument.Parse(handler.Body!);
        Assert.Equal("transactional", payload.RootElement.GetProperty("category").GetString());
        Assert.False(payload.RootElement.GetProperty("track_opens").GetBoolean());
        Assert.False(payload.RootElement.GetProperty("track_clicks").GetBoolean());
        Assert.Contains("123456", payload.RootElement.GetProperty("html").GetString());
        Assert.Contains("123456", payload.RootElement.GetProperty("text").GetString());
    }

    /// <summary>
    /// Verifies Recording Handler behavior.
    /// </summary>
    private sealed class RecordingHandler : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }
        public string? Body { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            Body = await request.Content!.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }
    }
}
