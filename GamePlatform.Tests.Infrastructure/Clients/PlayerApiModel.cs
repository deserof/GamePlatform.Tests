using System.Text.Json.Serialization;

namespace GamePlatform.Tests.Infrastructure.Clients;

/// <summary>
/// Real player payload (OpenAPI PlayerResponseDTO does not match the API).
/// Create returns <c>_id</c>; getOne/getAll return <c>id</c>.
/// </summary>
public sealed class PlayerApiModel
{
    [JsonPropertyName("_id")]
    public string? UnderscoreId { get; set; }

    [JsonPropertyName("id")]
    public string? PlainId { get; set; }

    [JsonIgnore]
    public string Id => UnderscoreId ?? PlainId ?? string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("surname")]
    public string Surname { get; set; } = string.Empty;

    [JsonPropertyName("currency_code")]
    public string? CurrencyCode { get; set; }
}
