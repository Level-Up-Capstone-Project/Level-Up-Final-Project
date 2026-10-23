using CapstoneProjectBE.Services;
using System.Net;
using System.Text;
using System.Text.Json;
using CapstoneProjectBE.ExternalServices.Transcripts;
using Microsoft.Extensions.Configuration;
using Xunit;

public class N8nTranscriptTests
{
    private static readonly byte[] Document = "%PDF-1.4 Official transcript binary"u8.ToArray();
    private static readonly StubTextExtractor Ocr = new("Mathematics 82% Programming 91% Official transcript of academic record");
    private static string Payload(object? score = null, string evidence = "Mathematics 82%") => JsonSerializer.Serialize(new
    {
        success = true, analysisType = "academic_transcript", analysis = new
        {
            academicScore = score ?? 86.5m, institution = "Example University", qualification = "BSc", summary = "Two modules analysed",
            modules = new[] {new {moduleName = "Mathematics", mark = 82, evidence}},
            detectedSkills = new[] {new {skillName = "Programming", evidence = "Programming 91%"}}, strengths = new[] {"Programming"},
        },
    });
    private static IConfiguration Config(string url = "https://example.com/webhook/transcript-analyze") => new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?>
    {
        ["N8N_TRANSCRIPT_WEBHOOK_URL"] = url, ["N8N_TRANSCRIPT_AUTH_HEADER_NAME"] = "X-LevelUp-API-Key", ["N8N_TRANSCRIPT_AUTH_HEADER_VALUE"] = "test-secret",
    }).Build();
    private static HttpResponseMessage Json(string payload) => new(HttpStatusCode.OK) {Content = new StringContent(payload, Encoding.UTF8, "application/json")};

    [Fact]
    public async Task SendsOcrTextAndMetadataWithAuthentication()
    {
        var handler = new Handler(async (request, ct) =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("https://example.com/webhook/transcript-analyze", request.RequestUri!.AbsoluteUri);
            Assert.Equal("test-secret", request.Headers.GetValues("X-LevelUp-API-Key").Single());
            using var body = JsonDocument.Parse(await request.Content!.ReadAsStringAsync(ct));
            Assert.Equal("Software Developer", body.RootElement.GetProperty("careerName").GetString());
            Assert.Equal("document.pdf", body.RootElement.GetProperty("fileName").GetString());
            Assert.Contains("Mathematics 82%", body.RootElement.GetProperty("transcriptText").GetString());
            return Json(Payload());
        });
        var result = await new N8nTranscriptAnalysisProvider(new N8nAnalysisService(new HttpClient(handler), Config()), Ocr)
            .AnalyseAsync("Software Developer", "transcript.pdf", "application/pdf", Document, default, ["Programming"]);
        Assert.Equal(86.5m, result.AcademicScore);
        Assert.Equal(82, result.Modules.Single().Mark);
        Assert.Equal("Programming", result.DetectedSkills.Single().SkillName);
    }

    [Fact]
    public async Task MergesCanonicalTextSkillsWhenN8nOmitsThem()
    {
        var ocr = new StubTextExtractor("Completed projects using C Sharp and RESTful APIs during software engineering studies.");
        var handler = new Handler((_, _) => Task.FromResult(Json(
            """{"success":true,"analysisType":"academic_transcript","analysis":{}}""")));
        var provider = new N8nTranscriptAnalysisProvider(new N8nAnalysisService(new HttpClient(handler), Config()), ocr);

        var result = await provider.AnalyseAsync(null, "transcript.pdf", "application/pdf", Document, default,
            ["C#", "REST APIs", "SQL"]);

        Assert.Equal(new[] {"C#", "REST APIs"}, result.DetectedSkills.Select(skill => skill.SkillName));
    }

    [Fact]
    public async Task RedactsPersonalDetailsAndFilenameBeforeSending()
    {
        var ocr = new StubTextExtractor("Email: jane@example.com Mathematics 82% Official transcript of academic record");
        var handler = new Handler(async (request, ct) =>
        {
            using var body = JsonDocument.Parse(await request.Content!.ReadAsStringAsync(ct));
            Assert.Equal("document.pdf", body.RootElement.GetProperty("fileName").GetString());
            Assert.DoesNotContain("jane@example.com", body.RootElement.GetProperty("transcriptText").GetString());
            return Json(Payload(evidence:"Mathematics 82%"));
        });
        var provider = new N8nTranscriptAnalysisProvider(new N8nAnalysisService(new HttpClient(handler), Config()), ocr);
        var result = await provider.AnalyseAsync(null, "Jane_Example_Transcript.pdf", "application/pdf", Document, default);
        Assert.Equal(82, result.Modules.Single().Mark);
    }

    [Fact]
    public void RejectsMalformedOrExplicitlyFailedResponses()
    {
        foreach (var payload in new[]
        {
            "{}", "[]", "{\"success\":false}",
            "{\"success\":true,\"analysisType\":\"cv\",\"analysis\":{}}",
            "[{\"statusCode\":502,\"response\":{\"success\":false}}]",
        })
        {
            using var json = JsonDocument.Parse(payload);
            Assert.Throws<N8nAnalysisException>(() => N8nTranscriptAnalysisProvider.Validate(json.RootElement));
        }
    }

    [Fact]
    public void AcceptsN8nItemEnvelopeAndOmitsUnavailableAnalysisFields()
    {
        const string payload = """
            [{"statusCode":200,"response":{"success":true,"analysisType":"academic_transcript","analysis":{
              "modules":[
                {"moduleName":"Mathematics","mark":82,"evidence":"Mathematics 82%"},
                {"moduleName":"Unverified module","mark":77}
              ],
              "detectedSkills":[{"skillName":"Programming"}]
            }}}]
            """;
        using var json = JsonDocument.Parse(payload);

        var result = N8nTranscriptAnalysisProvider.Validate(json.RootElement);

        Assert.Null(result.AcademicScore);
        Assert.Null(result.Institution);
        Assert.Null(result.Qualification);
        Assert.Equal(string.Empty, result.Summary);
        var module = Assert.Single(result.Modules);
        Assert.Equal("Mathematics", module.ModuleName);
        Assert.Equal(82, module.Mark);
        Assert.Empty(result.DetectedSkills);
        Assert.Empty(result.Strengths);
    }

    [Fact]
    public void AcceptsNestedCaseVariantResponseAndEntirelyMissingOptionalAnalysis()
    {
        const string payload = """
            {"body":[{"json":{"SUCCESS":true,"ANALYSISTYPE":"ACADEMIC_TRANSCRIPT","ANALYSIS":{
              "ACADEMICSCORE":"82.5",
              "MODULES":[
                {"MODULENAME":"Mathematics","MARK":"82%","EVIDENCE":"Mathematics 82%"},
                {"MODULEName":"Unsupported row","MARK":"not-a-number","EVIDENCE":"Unsupported row"}
              ],
              "DETECTEDSKILLS":[{"SKILLNAME":"Programming","EVIDENCE":"Programming module"}]
            }}}]}
            """;
        using var json = JsonDocument.Parse(payload);

        var result = N8nTranscriptAnalysisProvider.Validate(json.RootElement);

        Assert.Equal(82.5m, result.AcademicScore);
        Assert.Equal(2, result.Modules.Count);
        Assert.Equal("Mathematics", result.Modules[0].ModuleName);
        Assert.Equal(82, result.Modules[0].Mark);
        Assert.Null(result.Modules[1].Mark);
        Assert.Equal("Programming", Assert.Single(result.DetectedSkills).SkillName);
        Assert.Empty(result.Strengths);
    }

    [Fact]
    public void AcceptsSuccessfulAnalysisWithNoOptionalFields()
    {
        using var json = JsonDocument.Parse("""{"success":true,"analysisType":"academic_transcript","analysis":{}}""");

        var result = N8nTranscriptAnalysisProvider.Validate(json.RootElement);

        Assert.Null(result.AcademicScore);
        Assert.Null(result.Institution);
        Assert.Null(result.Qualification);
        Assert.Equal(string.Empty, result.Summary);
        Assert.Empty(result.Modules);
        Assert.Empty(result.DetectedSkills);
        Assert.Empty(result.Strengths);
    }

    [Fact]
    public void OmitsInvalidScoresInsteadOfRejectingTheTranscript()
    {
        using var json = JsonDocument.Parse(Payload(101).Replace("\"mark\":82", "\"mark\":40"));

        var result = N8nTranscriptAnalysisProvider.Validate(json.RootElement);

        Assert.Null(result.AcademicScore);
        Assert.Null(Assert.Single(result.Modules).Mark);
    }

    [Fact]
    public async Task RejectsNonProductionUrlsBeforeSendingAnything()
    {
        var handler = new Handler((_, _) => throw new Exception("Must not send"));
        foreach (var url in new[] {"http://example.com/webhook/transcript", "https://example.com/webhook-test/transcript", "https://user:secret@example.com/webhook/transcript"})
            Assert.Equal(503, (await Assert.ThrowsAsync<N8nAnalysisException>(() => new N8nTranscriptAnalysisProvider(new N8nAnalysisService(new HttpClient(handler), Config(url)), Ocr).AnalyseAsync(null, "transcript.pdf", "application/pdf", Document, default))).StatusCode);
    }

    [Fact]
    public async Task DoesNotExposeUpstreamErrorsOrTreatRedirectsAsResults()
    {
        foreach (var status in new[] {HttpStatusCode.Unauthorized, HttpStatusCode.Found, HttpStatusCode.InternalServerError})
        {
            var handler = new Handler((_, _) => Task.FromResult(new HttpResponseMessage(status) {Content = new StringContent("private workflow failure") }));
            var error = await Assert.ThrowsAsync<N8nAnalysisException>(() => new N8nTranscriptAnalysisProvider(new N8nAnalysisService(new HttpClient(handler), Config()), Ocr).AnalyseAsync(null, "transcript.pdf", "application/pdf", Document, default));
            Assert.DoesNotContain("private", error.Message);
            Assert.Equal(502, error.StatusCode);
            Assert.Equal((int)status, error.UpstreamStatusCode);
        }
    }

    [Fact]
    public async Task TimeoutIsDistinctFromCallerCancellation()
    {
        var handler = new Handler((_, _) => throw new TaskCanceledException());
        var provider = new N8nTranscriptAnalysisProvider(new N8nAnalysisService(new HttpClient(handler), Config()), Ocr);
        Assert.Equal(504, (await Assert.ThrowsAsync<N8nAnalysisException>(() => provider.AnalyseAsync(null, "transcript.pdf", "application/pdf", Document, default))).StatusCode);
        using var cancellation = new CancellationTokenSource(); cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => provider.AnalyseAsync(null, "transcript.pdf", "application/pdf", Document, cancellation.Token));
    }

    private static HttpContent Part(MultipartFormDataContent form, string name) =>
        form.Single(part => part.Headers.ContentDisposition?.Name?.Trim('"') == name);

    private sealed class Handler(Func<HttpRequestMessage,CancellationToken,Task<HttpResponseMessage>> send) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) => send(request, ct);
    }
}
