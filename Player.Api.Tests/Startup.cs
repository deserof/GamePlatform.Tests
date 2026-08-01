using GamePlatform.Tests.Infrastructure;
using GamePlatform.Tests.Steps;
using Microsoft.Extensions.DependencyInjection;
using Xunit.DependencyInjection.Logging;

namespace Player.Api.Tests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddInfrastructure();
        services.AddSteps();
        services.AddLogging(logging => logging.AddXunitOutput());
    }
}
