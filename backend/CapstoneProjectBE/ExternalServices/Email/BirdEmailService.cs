using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CapstoneProjectBE.ExternalServices.Email
{
    public sealed class BirdEmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BirdEmailService> _logger;

        public BirdEmailService(HttpClient httpClient, IConfiguration configuration, ILogger<BirdEmailService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendVerificationCodeAsync(string recipientEmail, string verificationCode, CancellationToken cancellationToken)
        {
            await SendCodeEmailAsync(recipientEmail, verificationCode, "LevelUp verification code",
                "Use this code to verify your LevelUp account.", cancellationToken);
        }

        public async Task SendPasswordResetCodeAsync(string recipientEmail, string verificationCode, CancellationToken cancellationToken)
        {
            await SendCodeEmailAsync(recipientEmail, verificationCode, "LevelUp password reset code",
                "Use this code to reset your LevelUp password.", cancellationToken);
        }

        public async Task SendPasswordChangedAsync(string recipientEmail, CancellationToken cancellationToken)
        {
            await SendEmailAsync(recipientEmail, "Your LevelUp password was changed",
                "Your LevelUp password was changed. If you did not make this change, contact LevelUp support immediately.",
                """
                <h2>Your LevelUp password was changed</h2>
                <p>Your password was changed successfully.</p>
                <p>If you did not make this change, contact LevelUp support immediately.</p>
                """, cancellationToken);
        }

        private async Task SendCodeEmailAsync(string recipientEmail, string verificationCode, string subject, string instruction, CancellationToken cancellationToken)
        {
            string encodedCode = WebUtility.HtmlEncode(verificationCode);

            await SendEmailAsync(recipientEmail, subject,
                $"LevelUp account security\n\n{instruction}\n\nYour code is: {verificationCode}\n\nThis code expires in 15 minutes. If you did not request this, you can ignore this email.",
                $"""
                    <h2>LevelUp account security</h2>
                    <p>{WebUtility.HtmlEncode(instruction)}</p>
                    <p>Your code is:</p>
                    <p style="font-size: 24px; font-weight: bold;">
                        {encodedCode}
                    </p>
                    <p>This code expires in 15 minutes. If you did not request this, you can ignore this email.</p>
                    """, cancellationToken);
        }

        private async Task SendEmailAsync(string recipientEmail, string subject, string text, string html, CancellationToken cancellationToken)
        {
            string apiKey = _configuration["Bird:ApiKey"] ?? throw new InvalidOperationException("The Bird API key is missing.");
            string fromEmail = _configuration["Bird:FromEmail"] ?? throw new InvalidOperationException("The Bird sender email is missing.");
            string fromName = _configuration["Bird:FromName"] ?? "LevelUp";

            var payload = new
            {
                @from = new
                {
                    email = fromEmail,
                    name = fromName
                },
                to = new[]
                {
                    recipientEmail
                },
                subject,
                text,
                html,
                category = "transactional",
                track_opens = false,
                track_clicks = false
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "v1/email/messages");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Content = JsonContent.Create(payload);

            using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogError("Bird rejected an account security email. Status: {StatusCode}. Response: {ResponseBody}", (int)response.StatusCode, responseBody);

                throw new HttpRequestException($"Bird email request failed with status code {(int)response.StatusCode}.");
            }

            _logger.LogInformation("Bird accepted an account security email for delivery.");
        }
    }
}
