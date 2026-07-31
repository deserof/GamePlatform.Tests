using System.Net.Http.Json;
using GamePlatform.Tests.Configuration;
using GamePlatform.Tests.Infrastructure.Auth;
using GamePlatform.Tests.Infrastructure.Clients;

namespace GamePlatform.Tests.Steps;

public class AuthSteps(IHttpClientFactory httpClientFactory, AuthTokenStore tokenStore)
{
    public Task<LoginResponse> LoginAsTesterAsync(CancellationToken cancellationToken = default)
    {
        var credentials = new CredentialsDTO
        {
            Email = TestConfiguration.Settings.Tester.Email,
            Password = TestConfiguration.Settings.Tester.Password,
        };

        return LoginAsync(credentials, cancellationToken);
    }

    public async Task<LoginResponse> LoginAsync(
        CredentialsDTO credentials,
        CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(nameof(IPlayerApiClient));
        using var response = await client.PostAsJsonAsync("api/tester/login", credentials, cancellationToken);
        response.EnsureSuccessStatusCode();

        var login = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Login response was empty.");

        if (string.IsNullOrWhiteSpace(login.AccessToken))
        {
            throw new InvalidOperationException("Login response did not contain accessToken.");
        }

        tokenStore.AccessToken = login.AccessToken;
        return login;
    }
}
