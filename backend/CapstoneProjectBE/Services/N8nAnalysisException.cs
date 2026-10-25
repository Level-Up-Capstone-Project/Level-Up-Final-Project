namespace CapstoneProjectBE.Services;

/// <summary>
/// Represents an N8n Analysis failure raised by an application service.
/// </summary>
public sealed class N8nAnalysisException(string message, int statusCode = 502, int? upstreamStatusCode = null) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public int? UpstreamStatusCode { get; } = upstreamStatusCode;
}
