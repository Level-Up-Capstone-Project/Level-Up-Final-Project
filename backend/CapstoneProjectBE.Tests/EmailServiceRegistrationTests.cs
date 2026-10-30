using CapstoneProjectBE.ExternalServices.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace CapstoneProjectBE.Tests;

/// <summary>
/// Verifies Email Service Registration behavior.
/// </summary>
public class EmailServiceRegistrationTests
{
    [Fact]
    public void DevelopmentProviderIsAvailableOnlyInDevelopment()
    {
        IConfiguration configuration = Configuration(new Dictionary<string, string?>
        {
            ["Email:Provider"] = "Development"
        });
        var environment = new TestEnvironment(Environments.Development);
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IHostEnvironment>(environment);

        services.AddLevelUpEmail(configuration, environment);

        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        Assert.IsType<DevelopmentEmailService>(serviceProvider.GetRequiredService<IEmailService>());

        var productionServices = new ServiceCollection();
        InvalidOperationException error = Assert.Throws<InvalidOperationException>(() =>
            productionServices.AddLevelUpEmail(configuration, new TestEnvironment(Environments.Production)));
        Assert.Contains("only be used in the Development environment", error.Message);
    }

    [Fact]
    public void BirdProviderFailsFastWhenApiKeyIsMissing()
    {
        IConfiguration configuration = Configuration(new Dictionary<string, string?>
        {
            ["Email:Provider"] = "Bird",
            ["Bird:BaseUrl"] = "https://eu1.platform.bird.com",
            ["Bird:FromEmail"] = "verify@example.com"
        });

        InvalidOperationException error = Assert.Throws<InvalidOperationException>(() =>
            new ServiceCollection().AddLevelUpEmail(
                configuration,
                new TestEnvironment(Environments.Production)));

        Assert.Equal("The Bird API key is missing.", error.Message);
    }

    [Fact]
    public void BirdProviderAcceptsCompleteConfiguration()
    {
        IConfiguration configuration = Configuration(new Dictionary<string, string?>
        {
            ["Email:Provider"] = "Bird",
            ["Bird:ApiKey"] = "test-key",
            ["Bird:BaseUrl"] = "https://eu1.platform.bird.com",
            ["Bird:FromEmail"] = "verify@example.com"
        });
        var services = new ServiceCollection();

        services.AddLevelUpEmail(configuration, new TestEnvironment(Environments.Production));

        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IEmailService));
    }

    [Fact]
    public async Task DevelopmentProviderNeverLogsOneTimeCodes()
    {
        var logger = new CapturingLogger<DevelopmentEmailService>();
        var service = new DevelopmentEmailService(logger, new TestEnvironment(Environments.Development));

        await service.SendVerificationCodeAsync("person@example.com", "123456", default);
        await service.SendPasswordResetCodeAsync("person@example.com", "654321", default);

        Assert.DoesNotContain(logger.Messages, message => message.Contains("123456", StringComparison.Ordinal));
        Assert.DoesNotContain(logger.Messages, message => message.Contains("654321", StringComparison.Ordinal));
        Assert.All(logger.Messages, message => Assert.Contains("simulated", message, StringComparison.OrdinalIgnoreCase));
    }

    private static IConfiguration Configuration(Dictionary<string, string?> values) =>
        new ConfigurationBuilder().AddInMemoryCollection(values).Build();

    /// <summary>
    /// Verifies Test Environment behavior.
    /// </summary>
    private sealed class TestEnvironment(string environmentName) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "CapstoneProjectBE.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    private sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) => Messages.Add(formatter(state, exception));
    }
}
