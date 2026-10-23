using System.Net;
using System.Text;
using CapstoneProjectBE.Controllers;
using CapstoneProjectBE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace CapstoneProjectBE.Tests;

public sealed class MiraWorkflowTests
{
    [Fact]
    public async Task ChatUsesConfiguredN8nWorkflowAndReturnsOnlyAssistantContent()
    {
        HttpMethod? sentMethod = null;
        string? sentApiKey = null;
        string? sentBody = null;
        var handler = new FakeHandler(request =>
        {
            sentMethod = request.Method;
            sentApiKey = request.Headers.GetValues("X-LevelUp-API-Key").Single();
            sentBody = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    """{"success":true,"message":{"content":"Practise with small coding exercises."}}""",
                    Encoding.UTF8, "application/json")
            };
        });
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["N8n:MiraUrl"] = "https://example.com/webhook/mira",
            ["N8n:AuthHeaderName"] = "X-LevelUp-API-Key",
            ["N8n:AuthHeaderValue"] = "test-secret"
        }).Build();
        var controller = new MiraController(new N8nAnalysisService(new HttpClient(handler), configuration));

        var result = await controller.Chat(new MiraController.ChatRequest("  How do I practise?  "), default);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Practise with small coding exercises.", ok.Value?.GetType().GetProperty("message")?.GetValue(ok.Value));
        Assert.Equal(HttpMethod.Post, sentMethod);
        Assert.Equal("test-secret", sentApiKey);
        Assert.Equal("""{"message":"How do I practise?"}""", sentBody);
    }

    private sealed class FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(respond(request));
    }
}
