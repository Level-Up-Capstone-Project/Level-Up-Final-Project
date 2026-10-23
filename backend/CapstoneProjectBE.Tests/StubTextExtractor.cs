using CapstoneProjectBE.ExternalServices.CVAnalysis;

internal sealed class StubTextExtractor(string text) : ICVTextExtractor
{
    public Task<string> ExtractTextAsync(byte[] fileBytes, string fileType, CancellationToken cancellationToken) =>
        Task.FromResult(text);
}
