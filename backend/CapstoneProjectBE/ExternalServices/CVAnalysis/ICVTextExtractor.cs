namespace CapstoneProjectBE.ExternalServices.CVAnalysis
{
    /// <summary>
    /// Defines the contract for the CVText Extractor integration.
    /// </summary>
    public interface ICVTextExtractor
    {
        Task<string> ExtractTextAsync(byte[] fileBytes, string fileType, CancellationToken cancellationToken);
    }
}
