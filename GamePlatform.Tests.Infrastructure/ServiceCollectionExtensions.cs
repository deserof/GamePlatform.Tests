using GamePlatform.Tests.Configuration;
using GamePlatform.Tests.Infrastructure.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace GamePlatform.Tests.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHttpClient<IPlayerApiClient, PlayerApiClient>(client =>
        {
            var baseUrl = TestConfiguration.Settings.PlayersApi.TrimEnd('/') + "/";
            client.BaseAddress = new Uri(baseUrl);
        });

        return services;
    }
}
