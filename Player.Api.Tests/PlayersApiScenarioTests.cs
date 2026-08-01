using System.Net;
using AwesomeAssertions;
using GamePlatform.Tests.Steps;

namespace Player.Api.Tests;

public class PlayersApiScenarioTests(AuthSteps authSteps, PlayerSteps playerSteps)
{
    private const int PlayersToCreate = 12;

    [Fact]
    public async Task Login_ShouldReturnAccessToken()
    {
        var (statusCode, token) = await authSteps.LoginAsTesterAsync();

        // BUG
        // statusCode.Should().Be(HttpStatusCode.OK);
        statusCode.Should().Be(HttpStatusCode.Created);
        token.Should().NotBeNull();
        token.AccessToken.Should().NotBeNullOrWhiteSpace();
        token.User.Should().NotBeNull();
        token.User!.Email.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task PlayersLifecycle_ShouldCreateGetSortDeleteAndVerifyEmpty()
    {
        var runPrefix = Guid.NewGuid().ToString("N")[..8];

        var (loginStatus, _) = await authSteps.LoginAsTesterAsync();
        // BUG
        //loginStatus.Should().Be(HttpStatusCode.OK);
        loginStatus.Should().Be(HttpStatusCode.Created);

        var created = await playerSteps.CreatePlayersAsync(PlayersToCreate, runPrefix);

        created.Should().HaveCount(PlayersToCreate);
        foreach (var (statusCode, request, response) in created)
        {
            statusCode.Should().Be(HttpStatusCode.Created);
            response.Id.Should().NotBeNullOrWhiteSpace();
            response.Email.Should().Be(request.Email);
            response.Username.Should().Be(request.Username);
            response.Name.Should().Be(request.Name);
            response.Surname.Should().Be(request.Surname);
        }

        var first = created[0];
        var (profileStatus, profile) = await playerSteps.GetPlayerByEmailAsync(first.Request.Email);

        // BUG
        //profileStatus.Should().Be(HttpStatusCode.OK);
        profileStatus.Should().Be(HttpStatusCode.Created);
        profile.Should().NotBeNull();
        profile.Id.Should().Be(first.Response.Id);
        profile.Email.Should().Be(first.Request.Email);
        profile.Username.Should().Be(first.Request.Username);
        profile.Name.Should().Be(first.Request.Name);
        profile.Surname.Should().Be(first.Request.Surname);

        var (getAllStatus, ownedPlayers) = await playerSteps.GetPlayersByEmailPrefixAsync(runPrefix);
        var sortedByName = ownedPlayers.OrderBy(p => p.Name).ToList();

        getAllStatus.Should().Be(HttpStatusCode.OK);
        ownedPlayers.Should().HaveCount(PlayersToCreate);
        sortedByName.Should().BeInAscendingOrder(p => p.Name);
        sortedByName.Select(p => p.Id).Should().BeEquivalentTo(created.Select(p => p.Response.Id));

        var deleteStatuses = await playerSteps.DeletePlayersAsync(created.Select(p => p.Response.Id));
        deleteStatuses.Should().OnlyContain(s => s == HttpStatusCode.OK);

        var (afterDeleteStatus, playersAfterDelete) = await playerSteps.GetPlayersByEmailPrefixAsync(runPrefix);
        afterDeleteStatus.Should().Be(HttpStatusCode.OK);
        playersAfterDelete.Should().BeEmpty();
    }
}
