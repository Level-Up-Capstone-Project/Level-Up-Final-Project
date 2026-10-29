using CapstoneProjectBE.Services;
using System.Security.Claims;
using System.Text.Json;
using CapstoneProjectBE.Controllers;
using CapstoneProjectBE.Data;
using CapstoneProjectBE.ExternalServices.Transcripts;
using CapstoneProjectBE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

/// <summary>
/// Verifies Transcript Controller behavior.
/// </summary>
public class TranscriptControllerTests
{
    [Fact]
    public async Task TranscriptAnalysisDoesNotCreateOrReplaceACv()
    {
        using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var id = Guid.NewGuid();
        db.Users.Add(new User {UserID = id, Email = "student@example.com", IsActive = true});
        db.UserProfiles.Add(new UserProfile {ProfileID = Guid.NewGuid(), UserID = id, FName = "Ada", LName = "Lovelace"});
        await db.SaveChangesAsync();
        var controller = new TranscriptsController(db, new Workflow())
        {
            ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext
            {User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, id.ToString())], "test"))}},
        };
        var pdf = Pdf();
        using var stream = new MemoryStream(pdf);
        var file = new FormFile(stream, 0, stream.Length, "file", "transcript.pdf")
        {Headers = new HeaderDictionary(), ContentType = "application/pdf"};
        var response = Assert.IsType<OkObjectResult>(await controller.Analyse(file, default));
        var json = JsonSerializer.SerializeToElement(response.Value, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.Equal(2, json.GetProperty("subjects").GetArrayLength());
        Assert.Equal(86.5m, json.GetProperty("averageMark").GetDecimal());
        Assert.Equal(82, json.GetProperty("subjects")[0].GetProperty("mark").GetDecimal());
        Assert.Empty(db.CVs);
        Assert.Empty(db.CVAnalyses);
        Assert.Single(db.TranscriptAnalyses);
        var saved = Assert.IsType<OkObjectResult>(await controller.Current(default));
        Assert.NotNull(saved.Value);
        var other = Guid.NewGuid();
        db.Users.Add(new User {UserID = other, Email = "other@example.com", IsActive = true});
        await db.SaveChangesAsync();
        controller.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, other.ToString())], "test"));
        Assert.IsType<NotFoundResult>(await controller.Current(default));
    }

    [Fact]
    public async Task TranscriptStreamsMilestonesBeforeResultAndReportsValidationErrors()
    {
        using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var id = Guid.NewGuid();
        db.Users.Add(new User {UserID = id, Email = "stream@example.com", IsActive = true});
        await db.SaveChangesAsync();
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, id.ToString())], "test")),
            RequestServices = new ServiceCollection().AddLogging().BuildServiceProvider()
        };
        context.Request.Headers.Accept = "application/x-ndjson";
        context.Response.Body = new MemoryStream();
        var controller = new TranscriptsController(db, new Workflow()) {ControllerContext = new ControllerContext {HttpContext = context}};
        var pdf = Pdf();
        using var input = new MemoryStream(pdf);
        var file = new FormFile(input, 0, input.Length, "file", "transcript.pdf") {Headers = new HeaderDictionary(), ContentType = "application/pdf"};
        Assert.IsType<EmptyResult>(await controller.Analyse(file, default));
        context.Response.Body.Position = 0;
        var lines = (await new StreamReader(context.Response.Body).ReadToEndAsync()).Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(line => JsonSerializer.Deserialize<JsonElement>(line)).ToArray();
        Assert.Equal(new[] {0, 15, 25, 65, 85, 95}, lines[..^1].Select(line => line.GetProperty("percent").GetInt32()));
        Assert.Equal("result", lines[^1].GetProperty("type").GetString());
        Assert.Equal(86.5m, lines[^1].GetProperty("data").GetProperty("averageMark").GetDecimal());
        Assert.Single(db.TranscriptAnalyses);
        var saved = Assert.IsType<OkObjectResult>(await controller.Current(default));
        var savedJson = JsonSerializer.SerializeToElement(saved.Value, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.Equal(lines[^1].GetProperty("data").GetProperty("analysisID").GetGuid(), savedJson.GetProperty("analysisID").GetGuid());
        context.Response.Body = new MemoryStream();
        var invalid = new FormFile(input, 0, input.Length, "file", "invalid.txt") {Headers = new HeaderDictionary(), ContentType = "text/plain"};
        await controller.Analyse(invalid, default);
        context.Response.Body.Position = 0;
        var errorLines = (await new StreamReader(context.Response.Body).ReadToEndAsync()).Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var error = JsonSerializer.Deserialize<JsonElement>(errorLines[^1]);
        Assert.Equal("error", error.GetProperty("type").GetString());
        Assert.Equal(400, error.GetProperty("status").GetInt32());
    }

    [Fact]
    public async Task WorkflowFailureDoesNotPersistASuccessfulReport()
    {
        using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var id = Guid.NewGuid();
        db.Users.Add(new User {UserID = id, Email = "failure@example.com", IsActive = true});
        await db.SaveChangesAsync();
        var controller = new TranscriptsController(db, new FailedWorkflow())
        {
            ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext
            {User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, id.ToString())], "test"))}},
        };
        using var stream = new MemoryStream(Pdf());
        var file = new FormFile(stream, 0, stream.Length, "file", "transcript.pdf") {Headers = new HeaderDictionary(), ContentType = "application/pdf"};
        var response = Assert.IsType<ObjectResult>(await controller.Analyse(file, default));
        Assert.Equal(502, response.StatusCode);
        Assert.Empty(db.TranscriptAnalyses);
    }

    /// <summary>
    /// Verifies Failed Workflow behavior.
    /// </summary>
    private sealed class FailedWorkflow : ITranscriptAnalysisProvider
    {
        public Task<TranscriptInsights> AnalyseAsync(string? careerName, string fileName, string contentType, byte[] document,
            CancellationToken ct, IReadOnlyList<string>? knownSkills = null, DocumentOwner? owner = null) =>
            throw new N8nAnalysisException("Workflow unavailable");
    }

    /// <summary>
    /// Verifies Workflow behavior.
    /// </summary>
    private sealed class Workflow : ITranscriptAnalysisProvider
    {
        public Task<TranscriptInsights> AnalyseAsync(string? careerName, string fileName, string contentType, byte[] document,
            CancellationToken ct, IReadOnlyList<string>? knownSkills = null, DocumentOwner? owner = null)
        {
            Assert.Equal("transcript.pdf", fileName);
            Assert.Equal("application/pdf", contentType);
            Assert.Equal(Pdf(), document);
            return Task.FromResult(new TranscriptInsights(86.5m, "University", "BSc", "Read two modules",
                [new("Mathematics", 82, "Mathematics 82%"), new("Computer Science", 91, "Computer Science 91%")], [], []));
        }
    }

    private static byte[] Pdf() => "%PDF-1.4 transcript document"u8.ToArray();
}
