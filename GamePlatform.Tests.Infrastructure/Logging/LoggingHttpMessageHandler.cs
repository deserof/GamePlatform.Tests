using System.Text.Json;
using System.Text.Json.Nodes;
using GamePlatform.Tests.Infrastructure.Reporting;
using Microsoft.Extensions.Logging;

namespace GamePlatform.Tests.Infrastructure.Logging;

public sealed class LoggingHttpMessageHandler(
    ILogger<LoggingHttpMessageHandler> logger,
    IReportStepTracer reportStepTracer)
    : DelegatingHandler
{
    private const string Redacted = "***";

    private static readonly HashSet<string> SensitiveKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "password",
        "password_change",
        "password_repeat",
        "accessToken",
        "access_token",
        "refreshToken",
        "refresh_token",
        "token",
    };

    private static readonly JsonSerializerOptions PrettyJson = new()
    {
        WriteIndented = true,
    };

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var path = request.RequestUri?.PathAndQuery ?? request.RequestUri?.ToString() ?? "(unknown)";
        var stepName = $"{request.Method} {path}";

        return reportStepTracer.RunStepAsync(stepName, async () =>
        {
            var requestBody = FormatBody(await ReadContentAsStringAsync(request.Content, cancellationToken));
            reportStepTracer.AttachText("request", requestBody);

            logger.LogInformation(
                "Sent {Method} to {Path}\nBody:\n{RequestBody}",
                request.Method,
                path,
                requestBody);

            var response = await base.SendAsync(request, cancellationToken);
            var responseBody = FormatBody(await ReadContentAsStringAsync(response.Content, cancellationToken));
            reportStepTracer.AttachText("response", $"Status: {(int)response.StatusCode}\n{responseBody}");

            logger.LogInformation(
                "Received status {StatusCode}\nBody:\n{ResponseBody}",
                (int)response.StatusCode,
                responseBody);

            return response;
        });
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
            var node = JsonNode.Parse(body);
            if (node is not null)
            {
                RedactSensitiveFields(node);
            }

            return node?.ToJsonString(PrettyJson) ?? body;
        }
        catch (JsonException)
        {
            return body;
        }
    }

    private static void RedactSensitiveFields(JsonNode node)
    {
        switch (node)
        {
            case JsonObject obj:
                foreach (var property in obj.ToList())
                {
                    if (SensitiveKeys.Contains(property.Key))
                    {
                        obj[property.Key] = Redacted;
                        continue;
                    }

                    if (property.Value is not null)
                    {
                        RedactSensitiveFields(property.Value);
                    }
                }

                break;

            case JsonArray array:
                foreach (var item in array)
                {
                    if (item is not null)
                    {
                        RedactSensitiveFields(item);
                    }
                }

                break;
        }
    }
}
