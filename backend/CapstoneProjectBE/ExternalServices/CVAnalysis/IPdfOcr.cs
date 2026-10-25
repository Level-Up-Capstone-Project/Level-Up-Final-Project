namespace CapstoneProjectBE.ExternalServices.CVAnalysis;

/// <summary>
/// Defines the contract for the Pdf Ocr integration.
/// </summary>
public interface IPdfOcr
{
    Task<IReadOnlyDictionary<int, string>> ReadPagesAsync(byte[] pdf, IReadOnlyList<int> pages, CancellationToken cancellationToken);
}
