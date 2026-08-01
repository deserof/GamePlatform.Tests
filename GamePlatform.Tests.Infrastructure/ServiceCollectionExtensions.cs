using GamePlatform.Tests.Configuration;
using GamePlatform.Tests.Infrastructure.Auth;
using GamePlatform.Tests.Infrastructure.Clients;
using GamePlatform.Tests.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GamePlatform.Tests.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
        
        services.AddLogging(logging =>
        {
            logging.AddSerilog(dispose: true);
            logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
        });

        services.AddSingleton<AuthTokenStore>();
        services.AddTransient<BearerTokenHandler>();
        services.AddTransient<LoggingHttpMessageHandler>();

        services.AddHttpClient<IPlayerApiClient, PlayerApiClient>(client =>
            {
                var baseUrl = TestConfiguration.Settings.PlayersApi.TrimEnd('/') + "/";
                client.BaseAddress = new Uri(baseUrl);
            })
            .AddHttpMessageHandler<BearerTokenHandler>()
            .AddHttpMessageHandler<LoggingHttpMessageHandler>();

        return services;
    }
}
