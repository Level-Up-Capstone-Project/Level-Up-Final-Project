namespace CapstoneProjectBE.ExternalServices.CVAnalysis
{
    public interface ICVTextExtractor
    {
        Task<string> ExtractTextAsync(byte[] fileBytes, string fileType, CancellationToken cancellationToken);
    }
}
