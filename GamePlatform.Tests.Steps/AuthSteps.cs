using System.Net;
using System.Net.Http.Json;
using GamePlatform.Tests.Configuration;
using GamePlatform.Tests.Infrastructure.Auth;
using GamePlatform.Tests.Infrastructure.Clients;
using GamePlatform.Tests.Steps.Models;

namespace GamePlatform.Tests.Steps;

public class AuthSteps(IHttpClientFactory httpClientFactory, AuthTokenStore tokenStore)
{
    public Task<(HttpStatusCode StatusCode, LoginResponse Body)> LoginAsTesterAsync(
        CancellationToken cancellationToken = default)
    {
        var credentials = new CredentialsDTO
        {
            Email = TestConfiguration.Settings.Tester.Email,
            Password = TestConfiguration.Settings.Tester.Password,
        };

        return LoginAsync(credentials, cancellationToken);
    }

    public async Task<(HttpStatusCode StatusCode, LoginResponse Body)> LoginAsync(
        CredentialsDTO credentials,
        CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(nameof(IPlayerApiClient));
        using var response = await client.PostAsJsonAsync("api/tester/login", credentials, cancellationToken);

        var login = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Login response was empty.");

        if (string.IsNullOrWhiteSpace(login.AccessToken))
        {
            throw new InvalidOperationException("Login response did not contain accessToken.");
        }

        tokenStore.AccessToken = login.AccessToken;
        return (response.StatusCode, login);
    }
}
