using CapstoneProjectBE.ExternalServices.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace CapstoneProjectBE.Tests;

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

    private static IConfiguration Configuration(Dictionary<string, string?> values) =>
        new ConfigurationBuilder().AddInMemoryCollection(values).Build();

    private sealed class TestEnvironment(string environmentName) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "CapstoneProjectBE.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
