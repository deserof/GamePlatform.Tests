using System.Net.Http.Json;
using GamePlatform.Tests.Infrastructure.Clients;
using GamePlatform.Tests.Infrastructure.Generators;

namespace GamePlatform.Tests.Steps;

public class PlayerSteps(IHttpClientFactory httpClientFactory)
{
    public async Task<(PlayerRequestDTO Request, PlayerApiModel Response)> CreatePlayerAsync(
        PlayerRequestDTO? request = null,
        CancellationToken cancellationToken = default)
    {
        request ??= new PlayerRequestFaker().Generate();

        var client = CreateClient();
        using var response = await client.PostAsJsonAsync("api/automationTask/create", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<PlayerApiModel>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Create player response was empty.");

        return (request, body);
    }

    public async Task<IReadOnlyList<(PlayerRequestDTO Request, PlayerApiModel Response)>> CreatePlayersAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        var players = new List<(PlayerRequestDTO, PlayerApiModel)>(count);

        for (var i = 0; i < count; i++)
        {
            players.Add(await CreatePlayerAsync(cancellationToken: cancellationToken));
        }

        return players;
    }

    public async Task<PlayerApiModel> GetPlayerByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var request = new PlayerRequestOneFaker(email).Generate();
        var client = CreateClient();
        using var response = await client.PostAsJsonAsync("api/automationTask/getOne", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PlayerApiModel>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Get player response was empty.");
    }

    public async Task<IReadOnlyList<PlayerApiModel>> GetAllPlayersAsync(
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        using var response = await client.GetAsync("api/automationTask/getAll", cancellationToken);
        response.EnsureSuccessStatusCode();

        var players = await response.Content
            .ReadFromJsonAsync<List<PlayerApiModel>>(cancellationToken: cancellationToken);

        return players ?? [];
    }

    public async Task DeletePlayerAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        using var response = await client.DeleteAsync($"api/automationTask/deleteOne/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeletePlayersAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
    {
        foreach (var id in ids)
        {
            await DeletePlayerAsync(id, cancellationToken);
        }
    }

    private HttpClient CreateClient() => httpClientFactory.CreateClient(nameof(IPlayerApiClient));
}
