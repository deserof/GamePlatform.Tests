using System.Text.Json.Serialization;

namespace GamePlatform.Tests.Steps.Models;

/// <summary>
/// Real login response (OpenAPI TokenDTO does not match the API).
/// </summary>
public sealed class LoginResponse
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public LoginUserResponse? User { get; set; }
}

public sealed class LoginUserResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("surname")]
    public string? Surname { get; set; }
}
