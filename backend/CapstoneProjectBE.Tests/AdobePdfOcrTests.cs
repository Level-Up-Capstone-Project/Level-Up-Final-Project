using System.Net;
using System.Net.Http.Json;
using CapstoneProjectBE.ExternalServices.CVAnalysis;
using Microsoft.Extensions.Configuration;
using Xunit;

public class AdobePdfOcrTests
{
    private static IConfiguration Config(bool credentials = true) => new ConfigurationBuilder().AddInMemoryCollection(
        credentials ? new Dictionary<string, string?> { ["Adobe:ClientId"] = "client", ["Adobe:ClientSecret"] = "secret" } : []).Build();

    [Fact]
    public async Task MissingCredentialsDoNotUploadDocuments()
    {
        var handler = new Handler(_ => throw new Exception("Unexpected network call"));
        await Assert.ThrowsAsync<InvalidDataException>(() => new AdobePdfOcr(new Factory(handler), Config(false)).ReadPagesAsync([], [0], default));
        Assert.Equal(0, handler.Calls);
    }

    [Fact]
    public async Task UploadsPollsAndExtractsRequestedPagesWithoutLeakingCredentialsToStorage()
    {
        var text = "Recognised Adobe OCR output with Python SQL and software engineering experience.";
        var handler = new Handler(request =>
        {
            var path = request.RequestUri!.AbsolutePath;
            if (request.RequestUri.Host.EndsWith("amazonaws.com"))
            {
                Assert.Null(request.Headers.Authorization);
                Assert.False(request.Headers.Contains("x-api-key"));
                return request.Method == HttpMethod.Put ? new HttpResponseMessage(HttpStatusCode.OK)
                    : new HttpResponseMessage(HttpStatusCode.OK) {Content = new ByteArrayContent(CVTextExtractorTests.Pdf(text))};
            }
            if (path == "/token") return Json(new {access_token = "adobe-token"});
            Assert.Equal("adobe-token", request.Headers.Authorization?.Parameter);
            Assert.Equal("client", request.Headers.GetValues("x-api-key").Single());
            if (path == "/assets") return Json(new {assetID = "asset", uploadUri = "https://dcplatformstorageservice-prod-us-east-1.s3-accelerate.amazonaws.com/input"});
            if (path == "/operation/ocr")
            {
                var response = new HttpResponseMessage(HttpStatusCode.Created);
                response.Headers.Location = new Uri("https://pdf-services-ue1.adobe.io/operation/ocr/job/status");
                return response;
            }
            return Json(new {status = "done", asset = new {downloadUri = "https://dcplatformstorageservice-prod-us-east-1.s3-accelerate.amazonaws.com/output"}});
        });
        var result = await new AdobePdfOcr(new Factory(handler), Config()).ReadPagesAsync(CVTextExtractorTests.Pdf(""), [0], default);
        Assert.Contains(text, result[0]);
        Assert.Equal(6, handler.Calls);
    }

    [Fact]
    public async Task RefusesUntrustedUploadLocations()
    {
        var handler = new Handler(request => request.RequestUri!.AbsolutePath == "/token"
            ? Json(new {access_token = "token"}) : Json(new {assetID = "asset", uploadUri = "http://127.0.0.1/private"}));
        await Assert.ThrowsAsync<InvalidDataException>(() => new AdobePdfOcr(new Factory(handler), Config()).ReadPagesAsync([], [0], default));
        Assert.Equal(2, handler.Calls);
    }

    [Fact]
    public async Task QuotaFailuresAreActionable()
    {
        var handler = new Handler(_ => new HttpResponseMessage(HttpStatusCode.TooManyRequests));
        var error = await Assert.ThrowsAsync<InvalidDataException>(() => new AdobePdfOcr(new Factory(handler), Config()).ReadPagesAsync([], [0], default));
        Assert.Contains("quota", error.Message);
    }

    private static HttpResponseMessage Json(object value) => new(HttpStatusCode.OK) { Content = JsonContent.Create(value) };
    private sealed class Handler(Func<HttpRequestMessage, HttpResponseMessage> send) : HttpMessageHandler
    {
        public int Calls { get; private set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        { Calls++; return Task.FromResult(send(request)); }
    }
    private sealed class Factory(HttpMessageHandler handler) : IHttpClientFactory
    { public HttpClient CreateClient(string name) => new(handler, disposeHandler: false); }
}
