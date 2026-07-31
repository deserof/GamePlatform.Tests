using GamePlatform.Tests.Steps;
using Microsoft.Extensions.DependencyInjection;

namespace GamePlatform.Tests.Steps;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSteps(this IServiceCollection services)
    {
        services.AddTransient<AuthSteps>();
        services.AddTransient<PlayerSteps>();
        return services;
    }
}
