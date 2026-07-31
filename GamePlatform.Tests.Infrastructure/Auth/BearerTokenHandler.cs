using System.Net.Http.Headers;

namespace GamePlatform.Tests.Infrastructure.Auth;

public sealed class BearerTokenHandler(AuthTokenStore tokenStore) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(tokenStore.AccessToken))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenStore.AccessToken);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
