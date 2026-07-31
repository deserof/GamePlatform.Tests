using AwesomeAssertions;
using GamePlatform.Tests.Steps;

namespace Player.Api.Tests;

public class PlayersApiScenarioTests(AuthSteps authSteps, PlayerSteps playerSteps)
{
    private const int PlayersToCreate = 12;

    [Fact]
    public async Task Login_ShouldReturnAccessToken()
    {
        var token = await authSteps.LoginAsTesterAsync();

        token.Should().NotBeNull();
        token.AccessToken.Should().NotBeNullOrWhiteSpace();
        token.User.Should().NotBeNull();
        token.User!.Email.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task PlayersLifecycle_ShouldCreateGetSortDeleteAndVerifyEmpty()
    {
        await authSteps.LoginAsTesterAsync();

        var preexisting = await playerSteps.GetAllPlayersAsync();
        if (preexisting.Count > 0)
        {
            await playerSteps.DeletePlayersAsync(preexisting.Select(p => p.Id));
        }

        var created = await playerSteps.CreatePlayersAsync(PlayersToCreate);

        created.Should().HaveCount(PlayersToCreate);
        foreach (var (request, response) in created)
        {
            response.Id.Should().NotBeNullOrWhiteSpace();
            response.Email.Should().Be(request.Email);
            response.Username.Should().Be(request.Username);
            response.Name.Should().Be(request.Name);
            response.Surname.Should().Be(request.Surname);
        }

        var first = created[0];
        var profile = await playerSteps.GetPlayerByEmailAsync(first.Request.Email);

        profile.Should().NotBeNull();
        profile.Id.Should().Be(first.Response.Id);
        profile.Email.Should().Be(first.Request.Email);
        profile.Username.Should().Be(first.Request.Username);
        profile.Name.Should().Be(first.Request.Name);
        profile.Surname.Should().Be(first.Request.Surname);

        var allPlayers = await playerSteps.GetAllPlayersAsync();
        var sortedByName = allPlayers.OrderBy(p => p.Name).ToList();

        allPlayers.Should().HaveCount(PlayersToCreate);
        sortedByName.Should().BeInAscendingOrder(p => p.Name);
        sortedByName.Select(p => p.Id).Should().BeEquivalentTo(created.Select(p => p.Response.Id));

        await playerSteps.DeletePlayersAsync(created.Select(p => p.Response.Id));

        var playersAfterDelete = await playerSteps.GetAllPlayersAsync();
        playersAfterDelete.Should().BeEmpty();
    }
}
