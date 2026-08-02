using System.Net;
using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using AwesomeAssertions;
using GamePlatform.Tests.Steps.Fixtures;
using GamePlatform.Tests.Steps.Steps;

namespace Player.Api.Tests.Tests;

[AllureEpic("Players API")]
[AllureFeature("Automation task")]
[AllureOwner("Artsiom Kharkevich")]
public class PlayersApiScenarioTests(
    AuthSteps authSteps,
    PlayerSteps playerSteps,
    PlayerTeardown playerTeardown) : IAsyncDisposable
{
    private const int PlayersToCreate = 12;

    public ValueTask DisposeAsync() => playerTeardown.DisposeAsync();

    [Fact]
    [AllureStory("Login")]
    [AllureSeverity(SeverityLevel.blocker)]
    [AllureIssue("BUG: /api/tester/login returns 201 instead of 200")]
    public async Task Login_ShouldReturnAccessToken()
    {
        var (statusCode, token) = await authSteps.LoginAsTesterAsync(refreshToken: true);

        statusCode.Should().Be(HttpStatusCode.OK);
        token.Should().NotBeNull();
        token.AccessToken.Should().NotBeNullOrWhiteSpace();
        token.User.Should().NotBeNull();
        token.User!.Email.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    [AllureStory("Get player profile")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureIssue("BUG: /api/automationTask/getOne returns 201 instead of 200")]
    public async Task GetPlayerProfile_ShouldReturnCreatedPlayer()
    {
        await authSteps.LoginAsTesterAsync();
        var (_, created) = await playerSteps.CreatePlayersAsync(1);
        var first = created[0];

        var (profileStatus, profile) = await playerSteps.GetPlayerByEmailAsync(first.Request.Email);

        profileStatus.Should().Be(HttpStatusCode.OK);
        profile.Should().NotBeNull();
        profile!.Id.Should().Be(first.Response.Id);
        profile.Email.Should().Be(first.Request.Email);
        profile.Username.Should().Be(first.Request.Username);
        profile.Name.Should().Be(first.Request.Name);
        profile.Surname.Should().Be(first.Request.Surname);
    }

    [Fact]
    [AllureStory("Get all players")]
    [AllureSeverity(SeverityLevel.critical)]
    public async Task GetAllPlayers_ShouldReturnOwnedPlayersSortedByName()
    {
        await authSteps.LoginAsTesterAsync();
        var (runPrefix, created) = await playerSteps.CreatePlayersAsync(PlayersToCreate);

        var (getAllStatus, players) = await playerSteps.GetPlayersByEmailPrefixAsync(runPrefix);
        // 4. Запросить данные всех пользователей и отсортировать их по имени (/api/automationTask/getAll)
        var sortedByName = players.OrderBy(p => p.Name).ToList();

        getAllStatus.Should().Be(HttpStatusCode.OK);
        players.Should().HaveCount(PlayersToCreate);
        sortedByName.Select(p => p.Id).Should().BeEquivalentTo(created.Select(p => p.Response.Id));
    }

    [Fact]
    [AllureStory("Create and delete players")]
    [AllureSeverity(SeverityLevel.critical)]
    public async Task CreateAndDeletePlayers_ShouldCreateThenRemoveAndLeaveEmptyList()
    {
        await authSteps.LoginAsTesterAsync();
        var (runPrefix, created) = await playerSteps.CreatePlayersAsync(PlayersToCreate);

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

        var deleteStatuses = await playerSteps.DeletePlayersAsync(created.Select(p => p.Response.Id));
        deleteStatuses.Should().OnlyContain(s => s == HttpStatusCode.OK);

        var (afterDeleteStatus, playersAfterDelete) = await playerSteps.GetPlayersByEmailPrefixAsync(runPrefix);
        afterDeleteStatus.Should().Be(HttpStatusCode.OK);
        playersAfterDelete.Should().BeEmpty();
    }
}
