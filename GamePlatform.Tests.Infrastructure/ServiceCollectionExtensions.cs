using GamePlatform.Tests.Configuration;
using GamePlatform.Tests.Infrastructure.Auth;
using GamePlatform.Tests.Infrastructure.Clients;
using GamePlatform.Tests.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace GamePlatform.Tests.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddTestLogging();

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
