using System.Net;
using GamePlatform.Tests.Infrastructure.Clients;
using GamePlatform.Tests.Steps.Models;

namespace GamePlatform.Tests.Steps.Fixtures;

public sealed class PlayerTestContext
{
    public string? RunPrefix { get; private set; }

    public IReadOnlyList<(HttpStatusCode StatusCode, PlayerRequestDTO Request, PlayerApiModel Response)> Created =>
        _created;

    private readonly List<(HttpStatusCode StatusCode, PlayerRequestDTO Request, PlayerApiModel Response)> _created = [];

    public void Track(
        string runPrefix,
        (HttpStatusCode StatusCode, PlayerRequestDTO Request, PlayerApiModel Response) player)
    {
        RunPrefix ??= runPrefix;
        _created.Add(player);
    }
}
