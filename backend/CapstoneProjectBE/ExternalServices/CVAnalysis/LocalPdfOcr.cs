using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace CapstoneProjectBE.ExternalServices.CVAnalysis;

/// <summary>Runs the isolated local OCR worker without a shell or document files on disk.</summary>
public sealed class LocalPdfOcr(IConfiguration configuration, IHostEnvironment environment, ILogger<LocalPdfOcr> logger) : IPdfOcr
{
    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task<IReadOnlyDictionary<int, string>> ReadPagesAsync(byte[] pdf, IReadOnlyList<int> pages, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try { return await RunAsync(pdf, pages, cancellationToken); }
        finally { _gate.Release(); }
    }

    private async Task<IReadOnlyDictionary<int, string>> RunAsync(byte[] pdf, IReadOnlyList<int> pages, CancellationToken cancellationToken)
    {
        var python = configuration["Ocr:PythonExecutable"];
        if (string.IsNullOrWhiteSpace(python))
        {
            var localPython = Path.GetFullPath(Path.Combine(environment.ContentRootPath, "..", ".venv-ocr",
                OperatingSystem.IsWindows() ? "Scripts/python.exe" : "bin/python"));
            python = File.Exists(localPython) ? localPython : OperatingSystem.IsWindows() ? "python" : "python3";
        }
        var script = Path.Combine(AppContext.BaseDirectory, "Ocr", "read_pdf.py");
        var start = new ProcessStartInfo(python)
        {
            UseShellExecute = false, CreateNoWindow = true,
            RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true,
            StandardInputEncoding = new UTF8Encoding(false), StandardOutputEncoding = Encoding.UTF8,
        };
        start.ArgumentList.Add(script);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(120));
        using var process = new Process { StartInfo = start };
        try
        {
            process.Start();
            var outputTask = process.StandardOutput.ReadToEndAsync(timeout.Token);
            var errorTask = process.StandardError.ReadToEndAsync(timeout.Token);
            var request = JsonSerializer.Serialize(new { pdf = Convert.ToBase64String(pdf), pages });
            await process.StandardInput.WriteAsync(request.AsMemory(), timeout.Token);
            process.StandardInput.Close();
            await process.WaitForExitAsync(timeout.Token);
            var output = await outputTask;
            await errorTask;
            if (process.ExitCode != 0)
            {
                logger.LogWarning("Local PDF OCR exited with code {ExitCode}", process.ExitCode);
                throw new InvalidDataException("OCR could not read this PDF. Check that the OCR dependencies are installed, or upload a text-based PDF or DOCX.");
            }
            var texts = JsonSerializer.Deserialize<Dictionary<int, string>>(output);
            if (texts is null || pages.Any(page => !texts.ContainsKey(page)))
                throw new InvalidDataException("OCR returned an incomplete result. Please try another PDF or DOCX.");
            return texts;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new InvalidDataException("OCR took too long. Try a shorter PDF or upload the original DOCX.");
        }
        catch (Win32Exception)
        {
            throw new InvalidDataException("OCR is not installed on the server. Configure Ocr:PythonExecutable and install the OCR requirements.");
        }
        finally
        {
            try { if (process.Id != 0 && !process.HasExited) process.Kill(entireProcessTree: true); }
            catch (InvalidOperationException) { /* The worker did not start or already exited. */ }
        }
    }
}
