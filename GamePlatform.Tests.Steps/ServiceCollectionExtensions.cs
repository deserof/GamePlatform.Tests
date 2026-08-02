using GamePlatform.Tests.Steps.Fixtures;
using GamePlatform.Tests.Steps.Steps;
using Microsoft.Extensions.DependencyInjection;

namespace GamePlatform.Tests.Steps;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSteps(this IServiceCollection services)
    {
        services.AddScoped<PlayerTestContext>();
        services.AddScoped<PlayerTeardown>();
        services.AddTransient<AuthSteps>();
        services.AddTransient<PlayerSteps>();
        return services;
    }
}
