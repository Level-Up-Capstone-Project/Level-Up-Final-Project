using System.Text;
using CapstoneProjectBE.ExternalServices.CVAnalysis;
using Xunit;

/// <summary>
/// Verifies CVText Extractor behavior.
/// </summary>
public class CVTextExtractorTests
{
    private const string NativeText = "Software developer with experience in CSharp SQL testing and building reliable web applications.";

    [Fact]
    public async Task TranscriptModeOcrsEveryPageIncludingSelectableText()
    {
        var ocr = new FakeOcr { Text = "Mathematics 82 percent and Computer Science 91 percent" };
        var result = await new CVTextExtractor(ocr, alwaysOcr: true).ExtractTextAsync(Pdf(NativeText, ""), "application/pdf", default);
        Assert.Equal(new[] {0, 1}, ocr.Requested);
        Assert.Contains(NativeText, result);
        Assert.EndsWith(ocr.Text, result);
    }

    [Fact]
    public async Task TranscriptModeDoesNotSkipOcrFailureForReadablePdf()
    {
        await Assert.ThrowsAsync<InvalidDataException>(() => new CVTextExtractor(new FakeOcr {Fail = true}, alwaysOcr: true)
            .ExtractTextAsync(Pdf(NativeText), "application/pdf", default));
    }

    [Fact]
    public async Task TextPdfDoesNotInvokeOcr()
    {
        var ocr = new FakeOcr();
        var result = await new CVTextExtractor(ocr).ExtractTextAsync(Pdf(NativeText), "application/pdf", default);
        Assert.Contains(NativeText, result);
        Assert.Empty(ocr.Requested);
    }

    [Fact]
    public async Task ScannedPageUsesOcrText()
    {
        var ocr = new FakeOcr { Text = NativeText };
        var result = await new CVTextExtractor(ocr).ExtractTextAsync(Pdf(""), "application/pdf", default);
        Assert.Equal(NativeText, result);
        Assert.Equal(new[] { 0 }, ocr.Requested);
    }

    [Fact]
    public async Task MixedPdfKeepsTextAndOnlyRecognisesScannedPage()
    {
        var ocr = new FakeOcr { Text = "Recognised second page" };
        var result = await new CVTextExtractor(ocr).ExtractTextAsync(Pdf(NativeText, ""), "application/pdf", default);
        Assert.Contains(NativeText, result);
        Assert.EndsWith(ocr.Text, result);
        Assert.Equal(new[] { 1 }, ocr.Requested);
    }

    [Fact]
    public async Task PoorOcrDoesNotDiscardExistingText()
    {
        var result = await new CVTextExtractor(new FakeOcr { Text = "" }).ExtractTextAsync(Pdf("Contact details"), "application/pdf", default);
        Assert.Contains("Contact details", result);
    }

    [Fact]
    public async Task CancellationPropagates()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => new CVTextExtractor(new FakeOcr()).ExtractTextAsync(Pdf(""), "application/pdf", cancellation.Token));
    }

    [Fact]
    public async Task PageLimitIsEnforcedBeforeOcr()
    {
        var ocr = new FakeOcr();
        await Assert.ThrowsAsync<InvalidDataException>(() => new CVTextExtractor(ocr).ExtractTextAsync(Pdf(Enumerable.Repeat("", 11).ToArray()), "application/pdf", default));
        Assert.Empty(ocr.Requested);
    }

    [Fact]
    public async Task OcrSetupErrorIsActionable()
    {
        var ocr = new FakeOcr { Fail = true };
        var error = await Assert.ThrowsAsync<InvalidDataException>(() => new CVTextExtractor(ocr).ExtractTextAsync(Pdf(""), "application/pdf", default));
        Assert.Equal("OCR dependencies missing", error.Message);
    }

    /// <summary>
    /// Verifies Fake Ocr behavior.
    /// </summary>
    private sealed class FakeOcr : IPdfOcr
    {
        public string Text { get; init; } = "";
        public bool Fail { get; init; }
        public int[] Requested { get; private set; } = [];
        public Task<IReadOnlyDictionary<int, string>> ReadPagesAsync(byte[] pdf, IReadOnlyList<int> pages, CancellationToken cancellationToken)
        {
            Requested = pages.ToArray();
            if (Fail) throw new InvalidDataException("OCR dependencies missing");
            return Task.FromResult<IReadOnlyDictionary<int, string>>(pages.ToDictionary(page => page, _ => Text));
        }
    }

    // Minimal valid PDF fixtures keep these tests independent of external rendering tools.
    internal static byte[] Pdf(params string[] pages)
    {
        var objects = new List<string> { "<< /Type /Catalog /Pages 2 0 R >>", "", "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>" };
        var kids = new List<string>();
        foreach (var text in pages)
        {
            var pageId = objects.Count + 1;
            kids.Add($"{pageId} 0 R");
            objects.Add($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 3 0 R >> >> /Contents {pageId + 1} 0 R >>");
            var content = $"BT /F1 12 Tf 20 700 Td ({text}) Tj ET";
            objects.Add($"<< /Length {content.Length} >>\nstream\n{content}\nendstream");
        }
        objects[1] = $"<< /Type /Pages /Kids [{string.Join(' ', kids)}] /Count {pages.Length} >>";
        var pdf = new StringBuilder("%PDF-1.4\n");
        var offsets = new List<int>();
        for (var i = 0; i < objects.Count; i++) { offsets.Add(pdf.Length); pdf.Append($"{i + 1} 0 obj\n{objects[i]}\nendobj\n"); }
        var xref = pdf.Length;
        pdf.Append($"xref\n0 {objects.Count + 1}\n0000000000 65535 f \n");
        foreach (var offset in offsets) pdf.Append($"{offset:D10} 00000 n \n");
        pdf.Append($"trailer\n<< /Size {objects.Count + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF");
        return Encoding.ASCII.GetBytes(pdf.ToString());
    }
}
