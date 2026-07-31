using AwesomeAssertions;
using GamePlatform.Tests.Configuration;
using GamePlatform.Tests.Infrastructure.Clients;

namespace Player.Api.Tests;

public class UnitTest1
{
    private readonly IPlayerApiClient _playerApiClient;

    public UnitTest1(IPlayerApiClient playerApiClient)
    {
        _playerApiClient = playerApiClient;
    }

    [Fact]
    public void TestConfiguration_ShouldLoadPlayersApiUrl()
    {
        var playersApi = TestConfiguration.Settings.PlayersApi;

        playersApi.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void PlayerApiClient_ShouldResolveFromDi()
    {
        _playerApiClient.Should().NotBeNull();
    }
}
