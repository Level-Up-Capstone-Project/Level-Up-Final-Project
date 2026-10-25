using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace CapstoneProjectBE.ExternalServices.CVAnalysis
{
    /// <summary>
    /// Provides CVText Extractor integration behavior.
    /// </summary>
    public class CVTextExtractor(IPdfOcr pdfOcr, bool alwaysOcr = false) : ICVTextExtractor
    {
        public async Task<string> ExtractTextAsync(byte[] fileBytes, string fileType, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return fileType.ToLowerInvariant() switch
                {
                    "application/pdf" => await ExtractPdfTextAsync(fileBytes, cancellationToken),
                    _ => throw new InvalidDataException("The CV file type is not supported.")
                };
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidDataException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new InvalidDataException("The CV document could not be read.", exception);
            }
        }

        private async Task<string> ExtractPdfTextAsync(byte[] fileBytes, CancellationToken cancellationToken)
        {
            var texts = new List<string>();
            var scanPages = new List<int>();
            using PdfDocument doc = PdfDocument.Open(fileBytes);

            if (doc.NumberOfPages > 10)
                throw new InvalidDataException("CV PDFs must contain no more than 10 pages.");

            foreach(var page in doc.GetPages())
            {
                cancellationToken.ThrowIfCancellationRequested();

                string pageText = ContentOrderTextExtractor.GetText(page);

                if (alwaysOcr || pageText.Count(char.IsLetterOrDigit) < 50) scanPages.Add(texts.Count);
                texts.Add(pageText);
            }
            if (scanPages.Count > 0)
            {
                var recognised = await pdfOcr.ReadPagesAsync(fileBytes, scanPages, cancellationToken);
                foreach (int index in scanPages)
                {
                    // Do not duplicate headers or replace embedded text with a worse OCR result.
                    if (recognised[index].Count(char.IsLetterOrDigit) > texts[index].Count(char.IsLetterOrDigit))
                        texts[index] = recognised[index];
                }
            }
            return string.Join(Environment.NewLine, texts);
        }

    }
}
