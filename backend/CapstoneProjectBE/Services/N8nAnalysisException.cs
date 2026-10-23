namespace CapstoneProjectBE.Services;

public sealed class N8nAnalysisException(string message, int statusCode = 502, int? upstreamStatusCode = null) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public int? UpstreamStatusCode { get; } = upstreamStatusCode;
}
