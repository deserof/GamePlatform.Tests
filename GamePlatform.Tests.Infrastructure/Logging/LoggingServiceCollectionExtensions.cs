using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Xunit.DependencyInjection.Logging;

namespace GamePlatform.Tests.Infrastructure.Logging;

public static class LoggingServiceCollectionExtensions
{
    public static IServiceCollection AddTestLogging(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        services.AddLogging(logging =>
        {
            logging.AddSerilog(dispose: true);
            logging.AddXunitOutput();
            logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
        });

        return services;
    }
}
