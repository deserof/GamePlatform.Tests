using GamePlatform.Tests.Infrastructure;
using GamePlatform.Tests.Steps;
using Microsoft.Extensions.DependencyInjection;

namespace Player.Api.Tests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddInfrastructure();
        services.AddSteps();
    }
}
