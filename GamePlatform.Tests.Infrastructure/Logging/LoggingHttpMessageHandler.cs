using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace GamePlatform.Tests.Infrastructure.Logging;

public sealed class LoggingHttpMessageHandler(ILogger<LoggingHttpMessageHandler> logger)
    : DelegatingHandler
{
    private static readonly JsonSerializerOptions PrettyJson = new()
    {
        WriteIndented = true,
    };

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var path = request.RequestUri?.PathAndQuery ?? request.RequestUri?.ToString() ?? "(unknown)";
        var requestBody = FormatBody(await ReadContentAsStringAsync(request.Content, cancellationToken));

        logger.LogInformation(
            "Sent {Method} to {Path}\nBody:\n{RequestBody}",
            request.Method,
            path,
            requestBody);

        var response = await base.SendAsync(request, cancellationToken);
        var responseBody = FormatBody(await ReadContentAsStringAsync(response.Content, cancellationToken));

        logger.LogInformation(
            "Received status {StatusCode}\nBody:\n{ResponseBody}",
            (int)response.StatusCode,
            responseBody);

        return response;
    }

    private static async Task<string> ReadContentAsStringAsync(
        HttpContent? content,
        CancellationToken cancellationToken)
    {
        if (content is null)
        {
            return string.Empty;
        }

        await content.LoadIntoBufferAsync(cancellationToken);
        return await content.ReadAsStringAsync(cancellationToken);
    }

    private static string FormatBody(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return "(empty)";
        }

        try
        {
            using var document = JsonDocument.Parse(body);
            return JsonSerializer.Serialize(document.RootElement, PrettyJson);
        }
        catch (JsonException)
        {
            return body;
        }
    }
}
