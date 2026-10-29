using System.Net;
using System.Text.Json;
using CapstoneProjectBE.ExternalServices.CVAnalysis;
using CapstoneProjectBE.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

/// <summary>
/// Verifies N8n Cv behavior.
/// </summary>
public class N8nCvTests
{
    private static readonly byte[] Document = "%PDF-1.4 CV binary"u8.ToArray();
    private static readonly StubTextExtractor Ocr = new("Software developer with SQL experience building reporting databases and APIs");
    private static CVAnalysisRequest Request() => new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Software Developer", "cv.pdf",
        "application/pdf", Document, [new(Guid.NewGuid(), "SQL")]);
    private static string Payload(CVAnalysisRequest request, decimal score = 78, string evidence = "SQL experience") => JsonSerializer.Serialize(new
    {
        success = true, analysisType = "cv", requestId = request.RequestId, cvID = request.CVID,
        analysis = new {qualityScore = score, atsScore = 80, formattingScore = 72, strengths = "Clear SQL experience", weaknesses = "Add outcomes",
            recommendations = "Quantify project outcomes", detectedSkills = new[] {new {skillName = "SQL", evidence}}},
    });

    [Fact]
    public async Task SharedServiceSendsCvTextAndServerIdsAndSupportsPerWorkflowAuth()
    {
        var request = Request();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?>
        {
            ["N8n:CvAnalysisUrl"] = "https://example.com/webhook/cv-analysis", ["N8n:AuthHeaderName"] = "X-ML-Admin-Key",
            ["N8n:AuthHeaderValue"] = "shared", ["N8n:CvAuthHeaderValue"] = "cv-specific",
        }).Build();
        var handler = new Handler(async message =>
        {
            Assert.Equal("https://example.com/webhook/cv-analysis", message.RequestUri!.AbsoluteUri);
            Assert.Equal("cv-specific", message.Headers.GetValues("X-ML-Admin-Key").Single());
            using var body = JsonDocument.Parse(await message.Content!.ReadAsStringAsync());
            var root = body.RootElement;
            Assert.Equal("document.pdf", root.GetProperty("fileName").GetString());
            Assert.Equal(request.CVID, root.GetProperty("cvID").GetGuid());
            Assert.Equal(request.RequestId, root.GetProperty("requestId").GetGuid());
            Assert.Equal(request.CareerName, root.GetProperty("careerName").GetString());
            Assert.Contains("SQL experience", root.GetProperty("cvText").GetString());
            return new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(Payload(request), System.Text.Encoding.UTF8, "application/json")};
        });
        var result = await Provider(handler, config).AnalyseAsync(request, default);
        Assert.Equal(78, result.QualityScore);
        Assert.Equal(request.KnownSkills[0].SkillID, result.DetectedSkills.Single().SkillID);
    }

    [Fact]
    public async Task MergesCanonicalTextMatchesWhenSuccessfulN8nResponseOmitsSkills()
    {
        var csharp = new CVSkillCandidate(Guid.NewGuid(), "C#");
        var rest = new CVSkillCandidate(Guid.NewGuid(), "REST APIs");
        var request = Request() with {KnownSkills = [csharp, rest]};
        var response = JsonSerializer.Serialize(new
        {
            success = true, analysisType = "cv", requestId = request.RequestId, cvID = request.CVID,
            analysis = new
            {
                qualityScore = 70, atsScore = 70, formattingScore = 70, strengths = "Clear experience",
                weaknesses = "Add outcomes", recommendations = "Quantify outcomes", detectedSkills = Array.Empty<object>()
            }
        });
        var handler = new Handler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {Content = new StringContent(response, System.Text.Encoding.UTF8, "application/json")}));
        var provider = new N8nCVAnalysisProvider(new N8nAnalysisService(new HttpClient(handler), Config()),
            new StubTextExtractor("Developed services using C Sharp and designed RESTful API integrations for clients."),
            new TextCVAnalysisProvider());

        var result = await provider.AnalyseAsync(request, default);

        Assert.Equal(new[] {csharp.SkillID, rest.SkillID}, result.DetectedSkills.Select(skill => skill.SkillID));
    }

    [Fact]
    public async Task RedactsFilenameAndPersonalDetails()
    {
        var original = Request() with
        {
            FileName = "Jane_Example_CV.pdf",
            Document = "%PDF-1.4 Jane Example jane@example.com SQL experience"u8.ToArray()
        };
        var handler = new Handler(async message =>
        {
            using var body = JsonDocument.Parse(await message.Content!.ReadAsStringAsync());
            Assert.Equal("document.pdf", body.RootElement.GetProperty("fileName").GetString());
            Assert.DoesNotContain("jane@example.com", body.RootElement.GetProperty("cvText").GetString());
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(Payload(original), System.Text.Encoding.UTF8, "application/json")
            };
        });
        var result = await new N8nCVAnalysisProvider(new N8nAnalysisService(new HttpClient(handler),
            new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?>
            {
                ["N8n:CvAnalysisUrl"] = "https://example.com/webhook/cv-analysis",
                ["N8n:AuthHeaderName"] = "X-LevelUp-API-Key",
                ["N8n:AuthHeaderValue"] = "test-secret",
            }).Build()), new StubTextExtractor("Jane Example Email: jane@example.com SQL experience building reporting databases"),
            new TextCVAnalysisProvider()).AnalyseAsync(original, default);
        Assert.Equal(78, result.QualityScore);
    }

    [Fact]
    public async Task RejectsAnotherUsersIdentifiedCvBeforeCallingN8n()
    {
        var calls = 0;
        var handler = new Handler(_ =>
        {
            calls++;
            throw new Exception("The workflow must not receive a rejected CV");
        });
        var provider = new N8nCVAnalysisProvider(new N8nAnalysisService(new HttpClient(handler), Config()),
            new StubTextExtractor("other.person@example.com\nExperienced software developer with SQL and API delivery experience."),
            new TextCVAnalysisProvider());
        var request = Request() with { Owner = new DocumentOwner("signed.in@example.com", "Signed", "In") };

        await Assert.ThrowsAsync<DocumentOwnershipException>(() => provider.AnalyseAsync(request, default));

        Assert.Equal(0, calls);
    }

    [Fact]
    public async Task RejectsCvWhoseHeaderDoesNotContainProfileNameBeforeCallingN8n()
    {
        var calls = 0;
        var provider = new N8nCVAnalysisProvider(new N8nAnalysisService(new HttpClient(new Handler(_ =>
        {
            calls++;
            throw new Exception("The workflow must not receive a rejected CV");
        })), Config()), new StubTextExtractor("Other Person\nSoftware developer with SQL and API delivery experience."),
            new TextCVAnalysisProvider());
        var request = Request() with { Owner = new DocumentOwner("signed.in@example.com", "Signed", "In") };

        await Assert.ThrowsAsync<DocumentOwnershipException>(() => provider.AnalyseAsync(request, default));

        Assert.Equal(0, calls);
    }

    [Fact]
    public async Task UsesTextFallbackWhenN8nReturnsAnError()
    {
        var request = Request();
        var text = string.Join('\n',
            "Summary", "developer@example.com 071 234 5678", "Skills SQL", "Experience",
            "Developed APIs, implemented reporting and improved database performance.", "Education", "Projects");
        var handler = new Handler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)));
        var result = await new N8nCVAnalysisProvider(new N8nAnalysisService(new HttpClient(handler), Config()),
            new StubTextExtractor(text), new TextCVAnalysisProvider()).AnalyseAsync(request, default);

        Assert.True(result.QualityScore > 0);
        Assert.Equal(request.KnownSkills[0].SkillID, result.DetectedSkills.Single().SkillID);
        Assert.Contains("recognised sections", result.Strengths);
    }

    [Fact]
    public async Task UsesTextFallbackWhenN8nIsNotConfigured()
    {
        var request = Request();
        var provider = new N8nCVAnalysisProvider(new N8nAnalysisService(new HttpClient(new Handler(_ =>
            throw new Exception("The unconfigured workflow must not make a request"))),
            new ConfigurationBuilder().Build()), Ocr, new TextCVAnalysisProvider());

        provider.EnsureConfigured();
        var result = await provider.AnalyseAsync(request, default);

        Assert.Equal(request.KnownSkills[0].SkillID, result.DetectedSkills.Single().SkillID);
    }

    [Fact]
    public void RejectsInvalidScoresAndMismatchedIds()
    {
        var request = Request();
        foreach (var payload in new[] {Payload(request, 101), Payload(Request()), "{}"})
        {
            using var json = JsonDocument.Parse(payload);
            Assert.Throws<N8nAnalysisException>(() => N8nCVAnalysisProvider.Validate(json.RootElement, request));
        }
    }

    [Fact]
    public void AcceptsCommonN8nEnvelopeAndCaseInsensitiveProperties()
    {
        var request = Request();
        var payload = JsonSerializer.Serialize(new[]
        {
            new
            {
                json = new
                {
                    Success = true,
                    AnalysisType = "CV",
                    RequestId = request.RequestId,
                    CvId = request.CVID,
                    Analysis = new
                    {
                        QualityScore = 78,
                        AtsScore = 80,
                        FormattingScore = 72,
                        Strengths = "Clear SQL experience",
                        Weaknesses = "Add outcomes",
                        Recommendations = "Quantify project outcomes",
                        DetectedSkills = new[] {new {SkillName = "SQL", Evidence = "SQL experience"}}
                    }
                }
            }
        });

        using var json = JsonDocument.Parse(payload);
        var result = N8nCVAnalysisProvider.Validate(json.RootElement, request);

        Assert.Equal(78, result.QualityScore);
        Assert.Equal(request.KnownSkills[0].SkillID, result.DetectedSkills.Single().SkillID);
    }

    private static HttpContent Part(MultipartFormDataContent form, string name) =>
        form.Single(part => part.Headers.ContentDisposition?.Name?.Trim('"') == name);

    private static IConfiguration Config() => new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["N8n:CvAnalysisUrl"] = "https://example.com/webhook/cv-analysis",
        ["N8n:AuthHeaderName"] = "X-LevelUp-API-Key",
        ["N8n:AuthHeaderValue"] = "test-secret",
    }).Build();

    private static N8nCVAnalysisProvider Provider(HttpMessageHandler handler, IConfiguration config) =>
        new(new N8nAnalysisService(new HttpClient(handler), config), Ocr, new TextCVAnalysisProvider());

    /// <summary>
    /// Verifies Handler behavior.
    /// </summary>
    private sealed class Handler(Func<HttpRequestMessage, Task<HttpResponseMessage>> send) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) => send(request);
    }
}
