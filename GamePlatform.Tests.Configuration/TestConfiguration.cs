using GamePlatform.Tests.Configuration.Models;
using Microsoft.Extensions.Configuration;

namespace GamePlatform.Tests.Configuration;

public static class TestConfiguration
{
    public const string UserSecretsId = "gameplatform-tests-player-api";

    private const string EnvironmentVariableName = "TEST_ENVIRONMENT";
    private const string DefaultEnvironment = "dev";

    private static readonly Lazy<IConfigurationRoot> Configuration = new(Build);

    public static IConfigurationRoot Current => Configuration.Value;

    public static string EnvironmentName =>
        Environment.GetEnvironmentVariable(EnvironmentVariableName) ?? DefaultEnvironment;

    public static TestSettings Settings =>
        Current.GetSection(TestSettings.SectionName).Get<TestSettings>()
        ?? throw new InvalidOperationException(
            $"Configuration section '{TestSettings.SectionName}' is missing or invalid.");

    private static IConfigurationRoot Build() =>
        new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile($"appsettings.{EnvironmentName}.json", optional: false)
            .AddUserSecrets(UserSecretsId)
            .AddEnvironmentVariables()
            .Build();
}
