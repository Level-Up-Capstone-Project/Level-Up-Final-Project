using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace CapstoneProjectBE.ExternalServices.CVAnalysis;

/// <summary>Adobe PDF Services asset upload, asynchronous OCR job, and searchable PDF extraction.</summary>
public sealed class AdobePdfOcr(IHttpClientFactory clients, IConfiguration config) : IPdfOcr
{
    private const string Api = "https://pdf-services.adobe.io";

    public async Task<IReadOnlyDictionary<int, string>> ReadPagesAsync(byte[] pdf, IReadOnlyList<int> pages, CancellationToken cancellationToken)
    {
        var clientId = config["Adobe:ClientId"];
        var secret = config["Adobe:ClientSecret"];
        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(secret))
            throw new InvalidDataException("Adobe OCR is not configured. Set Adobe:ClientId and Adobe:ClientSecret on the backend.");
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(120));
        var ct = timeout.Token;
        using var http = clients.CreateClient("AdobeOcr");
        try
        {
            using var credentials = new FormUrlEncodedContent(new Dictionary<string, string> { ["client_id"] = clientId, ["client_secret"] = secret });
            using var tokenResponse = await http.PostAsync(Api + "/token", credentials, ct);
            EnsureSuccess(tokenResponse);
            var token = (await tokenResponse.Content.ReadFromJsonAsync<JsonElement>(ct)).GetProperty("access_token").GetString()!;

            using var assetResponse = await SendApi(http, HttpMethod.Post, Api + "/assets", token, clientId, new { mediaType = "application/pdf" }, ct);
            var asset = await assetResponse.Content.ReadFromJsonAsync<JsonElement>(ct);
            var uploadUri = StorageUri(asset.GetProperty("uploadUri").GetString()!);
            using var upload = new HttpRequestMessage(HttpMethod.Put, uploadUri) { Content = new ByteArrayContent(pdf) };
            upload.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            using var uploaded = await http.SendAsync(upload, ct);
            EnsureSuccess(uploaded);

            using var job = await SendApi(http, HttpMethod.Post, Api + "/operation/ocr", token, clientId,
                new { assetID = asset.GetProperty("assetID").GetString() }, ct);
            var statusUri = job.Headers.Location ?? throw new InvalidDataException("Adobe OCR did not return a job location.");
            if (!statusUri.IsAbsoluteUri) statusUri = new Uri(new Uri(Api), statusUri);
            if (statusUri.Scheme != "https" ||
                !(statusUri.Host == "pdf-services.adobe.io" || statusUri.Host == "pdf-services-ue1.adobe.io" || statusUri.Host == "pdf-services-ew1.adobe.io") ||
                !statusUri.IsDefaultPort || statusUri.UserInfo.Length != 0)
                throw new InvalidDataException("Adobe OCR returned an invalid job location.");
            while (true)
            {
                using var poll = await SendApi(http, HttpMethod.Get, statusUri.AbsoluteUri, token, clientId, null, ct);
                var status = await poll.Content.ReadFromJsonAsync<JsonElement>(ct);
                var state = status.GetProperty("status").GetString();
                if (state == "failed") throw new InvalidDataException("Adobe OCR could not process this PDF. Try a clearer scan or the original DOCX.");
                if (state == "done")
                {
                    var download = StorageUri(status.GetProperty("asset").GetProperty("downloadUri").GetString()!);
                    using var result = await http.GetAsync(download, HttpCompletionOption.ResponseHeadersRead, ct);
                    EnsureSuccess(result);
                    await using var stream = await result.Content.ReadAsStreamAsync(ct);
                    using var bytes = new MemoryStream();
                    var buffer = new byte[81920];
                    int count;
                    while ((count = await stream.ReadAsync(buffer, ct)) > 0)
                    {
                        if (bytes.Length + count > 25 * 1024 * 1024) throw new InvalidDataException("Adobe OCR returned a document exceeding the result limit.");
                        await bytes.WriteAsync(buffer.AsMemory(0, count), ct);
                    }
                    using var document = PdfDocument.Open(bytes.ToArray());
                    if (pages.Any(page => page < 0 || page >= document.NumberOfPages))
                        throw new InvalidDataException("Adobe OCR returned an incomplete PDF.");
                    return pages.ToDictionary(page => page, page => ContentOrderTextExtractor.GetText(document.GetPage(page + 1)));
                }
                if (state != "in progress") throw new InvalidDataException("Adobe OCR returned an unexpected job status.");
                var delay = Math.Clamp(poll.Headers.RetryAfter?.Delta?.TotalSeconds ?? 1, 1, 5);
                await Task.Delay(TimeSpan.FromSeconds(delay), ct);
            }
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        { throw new InvalidDataException("Adobe OCR timed out. Please retry or use the original DOCX."); }
        catch (HttpRequestException)
        { throw new InvalidDataException("Adobe OCR is unavailable. Please retry later."); }
        catch (JsonException)
        { throw new InvalidDataException("Adobe OCR returned an unreadable response."); }
        catch (KeyNotFoundException)
        { throw new InvalidDataException("Adobe OCR returned an incomplete response."); }
        catch (InvalidOperationException)
        { throw new InvalidDataException("Adobe OCR returned an unexpected response."); }
    }

    private static async Task<HttpResponseMessage> SendApi(HttpClient http, HttpMethod method, string uri, string token, string clientId, object? body, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(method, uri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("x-api-key", clientId);
        if (body is not null) request.Content = JsonContent.Create(body);
        var response = await http.SendAsync(request, ct);
        try { EnsureSuccess(response); return response; }
        catch { response.Dispose(); throw; }
    }

    private static Uri StorageUri(string value)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || uri.Scheme != "https" || !uri.IsDefaultPort || uri.UserInfo.Length != 0 ||
            !(uri.Host == "dcplatformstorageservice-prod-us-east-1.s3-accelerate.amazonaws.com" ||
              uri.Host == "dcplatformstorageservice-prod-eu-west-1.s3.amazonaws.com"))
            throw new InvalidDataException("Adobe OCR returned an invalid asset location.");
        return uri;
    }

    private static void EnsureSuccess(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
            throw new InvalidDataException(response.StatusCode is System.Net.HttpStatusCode.Unauthorized or System.Net.HttpStatusCode.Forbidden
                ? "Adobe OCR credentials were rejected. Check the backend Adobe configuration."
                : response.StatusCode == System.Net.HttpStatusCode.TooManyRequests
                    ? "Adobe OCR quota or rate limit reached. Retry later or check the Adobe plan."
                    : "Adobe OCR could not complete the request. Please retry later.");
    }
}
