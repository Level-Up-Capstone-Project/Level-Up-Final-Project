using CapstoneProjectBE.ExternalServices.CVAnalysis;

/// <summary>
/// Verifies Stub Text Extractor behavior.
/// </summary>
internal sealed class StubTextExtractor(string text) : ICVTextExtractor
{
    public Task<string> ExtractTextAsync(byte[] fileBytes, string fileType, CancellationToken cancellationToken) =>
        Task.FromResult(text);
}
