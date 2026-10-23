using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace CapstoneProjectBE.ExternalServices.Storage
{
    public class CVStorageService : ICVStorageService
    {
        private const long MaxFileSizeBytes = 5 * 1024 * 1024;

        private static readonly Dictionary<string, string> AllowedFileTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            [".pdf"] = "application/pdf"
        };

        private readonly HttpClient _httpClient;
        private readonly ILogger<CVStorageService> _logger;
        private readonly string _supabaseUrl;
        private readonly string _secretKey;
        private readonly string _bucket;

        public CVStorageService(HttpClient httpClient, IConfiguration config, ILogger<CVStorageService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _supabaseUrl = config["Supabase:Url"]?.TrimEnd('/') ?? string.Empty;
            _secretKey = config["Supabase:SecretKey"] ?? string.Empty;
            _bucket = config["Supabase:CVBucket"] ?? string.Empty;

            if(string.IsNullOrWhiteSpace(_supabaseUrl))
            {
                throw new InvalidOperationException("Supabase URl is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_secretKey))
            {
                throw new InvalidOperationException("Supabase secret key is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_bucket))
            {
                throw new InvalidOperationException("Supabase CV bucket is not configured.");
            }
        }

        public async Task<string> UploadAsync(Guid userId, Guid cvId, IFormFile file, CancellationToken cancellationToken)
        {
            string contentType = await ValidateFileAsync(file, cancellationToken);
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string objPath = $"{userId:N}/{cvId:N}{extension}";
            string requestUrl = BuildObjectUrl(objPath);

            await using Stream fileStream = file.OpenReadStream();
            using var content = new StreamContent(fileStream);

            content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            content.Headers.ContentLength = file.Length;

            using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);

            request.Headers.TryAddWithoutValidation("apikey", _secretKey);
            request.Headers.TryAddWithoutValidation("x-upsert", "false");

            request.Content = content;

            using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead ,cancellationToken);

            if(!response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("CV upload failed. Status: {StatusCode}. Response: {Response}", (int)response.StatusCode, responseBody);
                throw new HttpRequestException("CV could not be saved to file storage.", null, response.StatusCode);
            }

            return objPath;
        }

        public async Task DeleteAsync(string objPath, CancellationToken cancellationToken)
        {
            string requestUrl = BuildObjectUrl(objPath);
            using var request = new HttpRequestMessage(HttpMethod.Delete, requestUrl);

            request.Headers.TryAddWithoutValidation("apikey", _secretKey);

            using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);

            if(response.StatusCode == HttpStatusCode.NotFound)
            {
                return;
            }

            if(!response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("CV deletion failed. Status: {StatusCode}. Response: {Response}", (int)response.StatusCode, responseBody);
                throw new HttpRequestException("CV could not be removed from file storage.", null, response.StatusCode);
            }
        }

        public async Task<byte[]> DownloadAsync(string objectPath, CancellationToken cancellationToken)
        {
            string requestUrl = BuildObjectUrl(objectPath);

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            request.Headers.TryAddWithoutValidation("apikey", _secretKey);

            using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FileNotFoundException("The stored CV file could not be found.");
            }

            if (!response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogError("CV download failed. Status: {StatusCode}. Response: {Response}", (int)response.StatusCode, responseBody);

                throw new HttpRequestException("The CV could not be downloaded from file storage.", null, response.StatusCode);
            }

            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }

        internal static async Task<string> ValidateFileAsync(IFormFile file, CancellationToken cancellationToken)
        {
            if(file.Length == 0)
            {
                throw new InvalidDataException("The file is empty.");
            }

            if(file.Length > MaxFileSizeBytes)
            {
                throw new InvalidDataException("The file cannot be larger than 5MB.");
            }

            string safeFileName = Path.GetFileName(file.FileName);

            if(string.IsNullOrWhiteSpace(safeFileName))
            {
                throw new InvalidDataException("The file name is invalid.");
            }

            string extension = Path.GetExtension(safeFileName).ToLowerInvariant();

            if(!AllowedFileTypes.TryGetValue(extension, out string? expectedContentType))
            {
                throw new InvalidDataException("Only PDF files are allowed.");
            }

            if(!string.Equals(file.ContentType, expectedContentType, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException("File type does not match its extension.");
            }

            if(file.Length < 5)
            {
                throw new InvalidDataException("The file is not valid");
            }

            byte[] header = new byte[5];

            await using Stream signatureStream = file.OpenReadStream();
            await signatureStream.ReadExactlyAsync(header, cancellationToken);

            bool validSignature = Encoding.ASCII.GetString(header) == "%PDF-";

            if(!validSignature)
            {
                throw new InvalidDataException("The file contents are not a valid PDF document.");
            }

            return expectedContentType;
        }

        private string BuildObjectUrl(string objPath)
        {
            string encodedBucket = Uri.EscapeDataString(_bucket);
            string encodedPath = string.Join("/", objPath.Split("/", StringSplitOptions.RemoveEmptyEntries).Select(Uri.EscapeDataString));

            return $"{_supabaseUrl}/storage/v1/object/{encodedBucket}/{encodedPath}";
        }
    }
}
