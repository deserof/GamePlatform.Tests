using System.Net;
using System.Net.Http.Json;
using GamePlatform.Tests.Infrastructure.Clients;
using GamePlatform.Tests.Infrastructure.Generators;
using GamePlatform.Tests.Steps.Fixtures;
using GamePlatform.Tests.Steps.Models;

namespace GamePlatform.Tests.Steps.Steps;

public class PlayerSteps(IHttpClientFactory httpClientFactory, PlayerTestContext playerTestContext)
{
    private HttpClient Client => httpClientFactory.CreateClient(nameof(IPlayerApiClient));

    public async Task<(HttpStatusCode StatusCode, PlayerRequestDTO Request, PlayerApiModel Response)> CreatePlayerAsync(
        PlayerRequestDTO? request = null,
        string? runPrefix = null,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            var faker = new PlayerRequestFaker(runPrefix);
            request = faker.Generate();
            runPrefix = faker.RunPrefix;
        }

        using var response = await Client.PostAsJsonAsync("api/automationTask/create", request, cancellationToken);

        var body = await response.Content.ReadFromJsonAsync<PlayerApiModel>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Create player response was empty.");

        var created = (response.StatusCode, request, body);
        if (!string.IsNullOrWhiteSpace(runPrefix))
        {
            playerTestContext.Track(runPrefix, created);
        }

        return created;
    }

    public async Task<(
        string RunPrefix,
        IReadOnlyList<(HttpStatusCode StatusCode, PlayerRequestDTO Request, PlayerApiModel Response)> Players)> CreatePlayersAsync(
        int count,
        string? runPrefix = null,
        CancellationToken cancellationToken = default)
    {
        var faker = new PlayerRequestFaker(runPrefix);
        var players = new List<(HttpStatusCode, PlayerRequestDTO, PlayerApiModel)>(count);

        for (var i = 0; i < count; i++)
        {
            players.Add(await CreatePlayerAsync(request: faker.Generate(), runPrefix: faker.RunPrefix, cancellationToken: cancellationToken));
        }

        return (faker.RunPrefix, players);
    }

    public async Task<(HttpStatusCode StatusCode, PlayerApiModel Body)> GetPlayerByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var request = new PlayerRequestOneFaker(email).Generate();
        using var response = await Client.PostAsJsonAsync("api/automationTask/getOne", request, cancellationToken);

        var body = await response.Content.ReadFromJsonAsync<PlayerApiModel>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Get player response was empty.");

        return (response.StatusCode, body);
    }

    public async Task<(HttpStatusCode StatusCode, IReadOnlyList<PlayerApiModel> Body)> GetAllPlayersAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await Client.GetAsync("api/automationTask/getAll", cancellationToken);

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
        using var response = await Client.DeleteAsync($"api/automationTask/deleteOne/{id}", cancellationToken);
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

    public async Task CleanupPlayersAsync(
        string? runPrefix,
        IEnumerable<(HttpStatusCode StatusCode, PlayerRequestDTO Request, PlayerApiModel Response)>? created = null,
        CancellationToken cancellationToken = default)
    {
        var ids = (created ?? [])
            .Select(p => p.Response.Id)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToHashSet(StringComparer.Ordinal);

        if (!string.IsNullOrWhiteSpace(runPrefix))
        {
            var (_, owned) = await GetPlayersByEmailPrefixAsync(runPrefix, cancellationToken);
            foreach (var id in owned.Select(p => p.Id).Where(id => !string.IsNullOrWhiteSpace(id)))
            {
                ids.Add(id);
            }
        }

        if (ids.Count == 0)
        {
            return;
        }

        await DeletePlayersAsync(ids, cancellationToken);
    }
}
