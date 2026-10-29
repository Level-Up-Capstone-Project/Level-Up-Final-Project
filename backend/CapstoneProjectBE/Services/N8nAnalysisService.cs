using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CapstoneProjectBE.Services;

/// <summary>Shared bounded, authenticated transport for the active production n8n workflows.</summary>
public sealed class N8nAnalysisService(HttpClient http, IConfiguration config, ILogger<N8nAnalysisService>? logger = null)
{
    public void EnsureConfigured(string workflow) => Configuration(workflow);

    private (Uri Uri, string Header, string Secret) Configuration(string workflow)
    {
        if (workflow is not ("Cv" or "Transcript" or "KnowledgeCheck" or "Roadmap" or "Mira")) throw new ArgumentOutOfRangeException(nameof(workflow));
        var prefix = workflow switch { "Cv" => "N8N_CV", "Transcript" => "N8N_TRANSCRIPT", "KnowledgeCheck" => "N8N_KNOWLEDGE_CHECK", "Mira" => "N8N_MIRA", _ => "N8N_ROADMAP" };
        var endpoint = config[workflow switch { "Cv" => "N8n:CvAnalysisUrl", "Transcript" => "N8n:TranscriptAnalysisUrl", "KnowledgeCheck" => "N8n:KnowledgeCheckUrl", "Mira" => "N8n:MiraUrl", _ => "N8n:RoadmapUrl" }] ?? config[$"{prefix}_WEBHOOK_URL"];
        var header = config[$"N8n:{workflow}AuthHeaderName"] ?? config[$"N8n:AuthHeaderName"] ?? config[$"{prefix}_AUTH_HEADER_NAME"];
        var secret = config[$"N8n:{workflow}AuthHeaderValue"] ?? config[$"N8n:AuthHeaderValue"] ?? config[$"{prefix}_AUTH_HEADER_VALUE"];
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri) || uri.Scheme != "https" || uri.UserInfo.Length != 0 ||
            uri.Query.Length != 0 || uri.Fragment.Length != 0 || !uri.AbsolutePath.StartsWith("/webhook/", StringComparison.Ordinal) ||
            string.IsNullOrWhiteSpace(header) || string.IsNullOrWhiteSpace(secret))
            throw new N8nAnalysisException($"{workflow} workflow is not configured on the server.", 503);
        if (!Regex.IsMatch(header, @"^[A-Za-z0-9!#$%&'*+.^_`|~-]+$") || secret.Contains('\r') || secret.Contains('\n') ||
            new[] {"Host", "Content-Type", "Content-Length", "Connection", "Transfer-Encoding", "Cookie"}.Contains(header, StringComparer.OrdinalIgnoreCase))
            throw new N8nAnalysisException("Analysis authentication is not configured correctly.", 503);
        return (uri, header, secret);
    }

    public Task<JsonElement> AnalyseCvAsync(Guid requestId, Guid cvID, Guid? careerID, string? careerName, string fileName, string cvText, CancellationToken ct) =>
        SendAsync("Cv", new {requestId, cvID, careerID, careerName, targetCareer = new {careerID, careerName}, fileName, cvText}, cvText, ct);

    public Task<JsonElement> ChatAsync(string message, CancellationToken ct) => SendAsync("Mira", new {message}, message, ct);

    public Task<JsonElement> AnalyseTranscriptAsync(string? careerName, string fileName, string transcriptText, CancellationToken ct) =>
        SendAsync("Transcript", new {careerName, fileName, transcriptText}, transcriptText, ct);

    public Task<JsonElement> GenerateKnowledgeCheckAsync(
        Guid requestId,
        Guid skillID,
        string skillName,
        N8nKnowledgeCheckCourse course,
        int attemptNumber,
        CancellationToken ct)
    {
        if (requestId == Guid.Empty || skillID == Guid.Empty || string.IsNullOrWhiteSpace(skillName) ||
            course.ResourceID == Guid.Empty || string.IsNullOrWhiteSpace(course.ResourceName) || attemptNumber < 1)
            throw new ArgumentException("A request ID, skill, completed course and positive attempt number are required.");
        return SendAsync("KnowledgeCheck", new
        {
            requestId,
            skillID,
            skillName,
            skill = new {skillID, skillName},
            course = new
            {
                resourceID = course.ResourceID,
                resourceName = course.ResourceName,
                provider = course.Provider,
                providerResourceID = course.ProviderResourceID,
                resourceType = course.ResourceType,
                description = course.Description
            },
            attemptNumber
        }, $"{skillName}\n{course.ResourceName}\n{course.Description}", ct);
    }

    public Task<JsonElement> GenerateRoadmapAsync(Guid requestId, Guid? careerID, string careerName, IReadOnlyList<N8nRoadmapSkill> skills, CancellationToken ct)
    {
        if (requestId == Guid.Empty || string.IsNullOrWhiteSpace(careerName) || skills is null || skills.Count == 0 ||
            skills.Any(s => s is null || s.SkillID == Guid.Empty || string.IsNullOrWhiteSpace(s.SkillName) ||
                s.Status is not ("Missing" or "Partial" or "Have") || s.ResourceSlots is < 0 or > 3 ||
                s.ExistingResources is { Count: > 3 }))
            throw new ArgumentException("A request ID, target career and valid skills are required.");
        return SendAsync("Roadmap", new {requestId, targetCareer = new {careerID, careerName}, skills}, careerName, ct);
    }

    private async Task<JsonElement> SendAsync(string workflow, object payload, string text, CancellationToken ct)
    {
        var (uri, header, secret) = Configuration(workflow);
        if (string.IsNullOrWhiteSpace(text) || text.Length > 200_000)
            throw new InvalidDataException("The workflow input is empty or exceeds the processing limit.");
        using var request = new HttpRequestMessage(HttpMethod.Post, uri) {Content = JsonContent.Create(payload)};
        request.Headers.Add(header, secret);
        request.Headers.Accept.ParseAdd("application/json");
        return await SendRequestAsync(workflow, request, ct);
    }

    private async Task<JsonElement> SendRequestAsync(string workflow, HttpRequestMessage request, CancellationToken ct)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        var defaultTimeout = workflow is "Cv" or "Transcript" ? 600 : 120;
        var configuredTimeout = config.GetValue<int?>("N8n:TimeoutSeconds");
        timeout.CancelAfter(TimeSpan.FromSeconds(configuredTimeout is > 0 and <= 3600 ? configuredTimeout.Value : defaultTimeout));
        try
        {
            using var response = await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeout.Token);
            // Keep the real upstream status for diagnostics, without forwarding workflow output or credentials.
            if (!response.IsSuccessStatusCode)
            {
                logger?.LogWarning("n8n {Workflow} workflow returned HTTP {StatusCode}.", workflow, (int)response.StatusCode);
                throw new N8nAnalysisException("The analysis service could not complete the request.", 502, (int)response.StatusCode);
            }
            if (!(response.Content.Headers.ContentType?.MediaType?.Contains("json", StringComparison.OrdinalIgnoreCase) ?? false))
                throw new N8nAnalysisException("The analysis service returned an unexpected response.");
            if (response.Content.Headers.ContentLength > 1024 * 1024) throw new N8nAnalysisException("The analysis response was too large.");
            await using var stream = await response.Content.ReadAsStreamAsync(timeout.Token);
            using var bytes = new MemoryStream();
            var buffer = new byte[8192];
            int read;
            while ((read = await stream.ReadAsync(buffer, timeout.Token)) > 0)
            {
                if (bytes.Length + read > 1024 * 1024) throw new N8nAnalysisException("The analysis response was too large.");
                await bytes.WriteAsync(buffer.AsMemory(0, read), timeout.Token);
            }
            using var json = JsonDocument.Parse(bytes.ToArray(), new JsonDocumentOptions {MaxDepth = 24});
            return json.RootElement.Clone();
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        { throw new N8nAnalysisException("Analysis took too long. Please try again.", 504); }
        catch (HttpRequestException error)
        {
            logger?.LogWarning(error, "Could not reach the n8n {Workflow} workflow.", workflow);
            throw new N8nAnalysisException("The analysis service is unavailable. Please try again.");
        }
        catch (JsonException error)
        {
            logger?.LogWarning(error, "n8n {Workflow} workflow returned invalid JSON.", workflow);
            throw new N8nAnalysisException("The analysis service returned invalid JSON.");
        }
    }
}

/// <summary>Trusted server-side context for the specific recommended course being assessed.</summary>
public sealed record N8nKnowledgeCheckCourse(
    Guid ResourceID,
    string ResourceName,
    string? Provider,
    string? ProviderResourceID,
    string? ResourceType,
    string? Description);
