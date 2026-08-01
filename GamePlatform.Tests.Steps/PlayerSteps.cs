using System.Net;
using System.Net.Http.Json;
using GamePlatform.Tests.Infrastructure.Clients;
using GamePlatform.Tests.Infrastructure.Generators;
using GamePlatform.Tests.Steps.Models;

namespace GamePlatform.Tests.Steps;

public class PlayerSteps(IHttpClientFactory httpClientFactory)
{
    public async Task<(HttpStatusCode StatusCode, PlayerRequestDTO Request, PlayerApiModel Response)> CreatePlayerAsync(
        PlayerRequestDTO? request = null,
        string? runPrefix = null,
        CancellationToken cancellationToken = default)
    {
        request ??= new PlayerRequestFaker(runPrefix).Generate();

        var client = CreateClient();
        using var response = await client.PostAsJsonAsync("api/automationTask/create", request, cancellationToken);

        var body = await response.Content.ReadFromJsonAsync<PlayerApiModel>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Create player response was empty.");

        return (response.StatusCode, request, body);
    }

    public async Task<IReadOnlyList<(HttpStatusCode StatusCode, PlayerRequestDTO Request, PlayerApiModel Response)>> CreatePlayersAsync(
        int count,
        string? runPrefix = null,
        CancellationToken cancellationToken = default)
    {
        var players = new List<(HttpStatusCode, PlayerRequestDTO, PlayerApiModel)>(count);

        for (var i = 0; i < count; i++)
        {
            players.Add(await CreatePlayerAsync(runPrefix: runPrefix, cancellationToken: cancellationToken));
        }

        return players;
    }

    public async Task<(HttpStatusCode StatusCode, PlayerApiModel Body)> GetPlayerByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var request = new PlayerRequestOneFaker(email).Generate();
        var client = CreateClient();
        using var response = await client.PostAsJsonAsync("api/automationTask/getOne", request, cancellationToken);

        var body = await response.Content.ReadFromJsonAsync<PlayerApiModel>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Get player response was empty.");

        return (response.StatusCode, body);
    }

    public async Task<(HttpStatusCode StatusCode, IReadOnlyList<PlayerApiModel> Body)> GetAllPlayersAsync(
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        using var response = await client.GetAsync("api/automationTask/getAll", cancellationToken);

        var players = await response.Content
            .ReadFromJsonAsync<List<PlayerApiModel>>(cancellationToken: cancellationToken);

        return (response.StatusCode, players ?? []);
    }

    public async Task<(HttpStatusCode StatusCode, IReadOnlyList<PlayerApiModel> Body)> GetPlayersByEmailPrefixAsync(
        string runPrefix,
        CancellationToken cancellationToken = default)
    {
        var (statusCode, all) = await GetAllPlayersAsync(cancellationToken);
        var filtered = all
            .Where(p => p.Email.StartsWith($"{runPrefix}.", StringComparison.OrdinalIgnoreCase)
                        && p.Email.EndsWith($"@{PlayerRequestFaker.EmailDomain}", StringComparison.OrdinalIgnoreCase))
            .ToList();

        return (statusCode, filtered);
    }

    public async Task<HttpStatusCode> DeletePlayerAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        using var response = await client.DeleteAsync($"api/automationTask/deleteOne/{id}", cancellationToken);
        return response.StatusCode;
    }

    public async Task<IReadOnlyList<HttpStatusCode>> DeletePlayersAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
    {
        var statuses = new List<HttpStatusCode>();
        foreach (var id in ids)
        {
            statuses.Add(await DeletePlayerAsync(id, cancellationToken));
        }

        return statuses;
    }

    private HttpClient CreateClient() => httpClientFactory.CreateClient(nameof(IPlayerApiClient));
}
