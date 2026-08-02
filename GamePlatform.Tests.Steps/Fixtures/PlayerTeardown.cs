using GamePlatform.Tests.Steps.Steps;

namespace GamePlatform.Tests.Steps.Fixtures;

public sealed class PlayerTeardown(PlayerSteps playerSteps, PlayerTestContext context) : IAsyncDisposable
{
    public ValueTask DisposeAsync() =>
        new(playerSteps.CleanupPlayersAsync(context.RunPrefix, context.Created));
}
