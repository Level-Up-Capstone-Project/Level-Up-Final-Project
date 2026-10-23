using System.Net.Mail;

namespace CapstoneProjectBE.ExternalServices.Email
{
    public static class EmailServiceRegistration
    {
        public static IServiceCollection AddLevelUpEmail(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment)
        {
            string provider = configuration["Email:Provider"]
                ?? (environment.IsDevelopment() ? "Development" : "Bird");

            if (provider.Equals("Development", StringComparison.OrdinalIgnoreCase))
            {
                if (!environment.IsDevelopment())
                {
                    throw new InvalidOperationException(
                        "The development email provider can only be used in the Development environment.");
                }

                services.AddSingleton<IEmailService, DevelopmentEmailService>();
                return services;
            }

            if (!provider.Equals("Bird", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Unsupported email provider '{provider}'. Use 'Development' or 'Bird'.");
            }

            string apiKey = Require(configuration, "Bird:ApiKey", "The Bird API key is missing.");
            string fromEmail = Require(configuration, "Bird:FromEmail", "The Bird sender email is missing.");
            string baseUrl = Require(configuration, "Bird:BaseUrl", "The Bird base URL is missing.");

            if (!MailAddress.TryCreate(fromEmail, out _))
            {
                throw new InvalidOperationException("The Bird sender email is invalid.");
            }

            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out Uri? baseUri) ||
                baseUri.Scheme != Uri.UriSchemeHttps ||
                !string.IsNullOrEmpty(baseUri.UserInfo) ||
                !string.IsNullOrEmpty(baseUri.Query) ||
                !string.IsNullOrEmpty(baseUri.Fragment))
            {
                throw new InvalidOperationException(
                    "The Bird base URL must be an HTTPS URL without credentials, a query, or a fragment.");
            }

            services.AddHttpClient<IEmailService, BirdEmailService>(client =>
            {
                client.BaseAddress = new Uri($"{baseUri.AbsoluteUri.TrimEnd('/')}/");
                client.Timeout = TimeSpan.FromSeconds(15);
            });

            return services;
        }

        private static string Require(
            IConfiguration configuration,
            string key,
            string errorMessage)
        {
            string? value = configuration[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return value.Trim();
        }
    }
}
