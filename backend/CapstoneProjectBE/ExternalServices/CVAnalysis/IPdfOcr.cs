namespace CapstoneProjectBE.ExternalServices.CVAnalysis;

public interface IPdfOcr
{
    Task<IReadOnlyDictionary<int, string>> ReadPagesAsync(byte[] pdf, IReadOnlyList<int> pages, CancellationToken cancellationToken);
}
